namespace ServiceLayer.SimulationKernel
{
  using DomainModel.SimulationKernel;
  using Generated;
  using Google.Protobuf;
  using Grpc.Core;
  using Grpc.Net.Client;
  using Microsoft.Extensions.Logging;
  using static Generated.FileTransfer;
  using ProcessingStatus = Generated.ProcessingStatus;
  using TransferStatus = Generated.TransferStatus;

  internal class TransferDataService : ITransferDataService
  {
    private static readonly string _FileExtension = ".obj";
    private static readonly int _ChunkSize = 10 * 1024;//10KB buffer

    private readonly ILogger<TransferDataService> _Logger;
    private readonly FileTransferClient _Client;

    public TransferDataService(ILogger<TransferDataService> logger)
    {
      _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
      var channel = CreateChannel("http://localhost:50051");
      _Client = new FileTransferClient(channel);
    }

    #region Upload
    public async Task<TransferStatus> UploadAsync(FileData file, IProgress<uint> progress)
    {
      TransferStatus status = TransferStatus.Pending;
      try
      {
        using var transfer = _Client.Transfer();
        await SendMetadata(transfer, file.Name, file.Destination);

        int bytesRead = 0, totalRead = 0;

        var buffer = new byte[_ChunkSize];
        while ((bytesRead = await file.ReadStream.ReadAsync(buffer)) != 0)
        {
          totalRead += bytesRead;
          await SendFileChunk(transfer, bytesRead, buffer);
          uint progressPercentage = (uint)(decimal.Divide(totalRead, file.Size) * 100);
          progress.Report(progressPercentage);
        }

        await transfer.RequestStream.CompleteAsync();
        var response = await transfer;
        status = response.Status;
      }
      catch (Exception ex)
      {
        _Logger.LogWarning(ex, "Error uploading file");
      }
      return status;
    }

    private static Task SendFileChunk(AsyncClientStreamingCall<FileTransferRequest, FileTransferResponse> transfer, int bytesRead, byte[] buffer)
    {
      return transfer.RequestStream.WriteAsync(new FileTransferRequest()
      {
        File = new File()
        {
          //Prefer over ByteString.CopyFrom
          Chunk = UnsafeByteOperations.UnsafeWrap(buffer.AsMemory(0, bytesRead))
        }
      });
    }

    private static Task SendMetadata(AsyncClientStreamingCall<FileTransferRequest, FileTransferResponse> transfer, string name, string destination)
    {
      return transfer.RequestStream.WriteAsync(new FileTransferRequest()
      {
        Metadata = new FileMetaData()
        {
          Name = Path.GetFileNameWithoutExtension(name),
          Extension = _FileExtension,
          Destination = destination
        }
      });
    }
    #endregion

    #region Download
    public async Task<TransferStatus> DownloadAsync(string location, IProgress<uint> progress, Action<(byte[] bytes, string file)> onFileReceived)
    {
      TransferStatus status = TransferStatus.Pending;
      try
      {
        using var call = _Client.Download(new ProcessingMetaData
        {
          DataLocation = location
        });

        string downloadLocation = Path.Combine("downloaded", Path.GetFileName(location));
        Directory.CreateDirectory(downloadLocation);

        var fileBytes = new List<byte>();

        await foreach (FileDownloadResponse response in call.ResponseStream.ReadAllAsync())
        {
          switch (response.ResponseCase)
          {
            case FileDownloadResponse.ResponseOneofCase.Metadata:
              {
                string fileName = Path.ChangeExtension(response.Metadata.Name, response.Metadata.Extension);
                string filePath = Path.Combine(downloadLocation, fileName);
                if (fileBytes.Any())
                {
                  onFileReceived?.Invoke((fileBytes.ToArray(), fileName));
                  fileBytes.Clear();
                }
              }
              break;
            case FileDownloadResponse.ResponseOneofCase.File:
              var bytes = response.File.Chunk.Memory;
              fileBytes.AddRange(bytes.ToArray());
              break;
            default:
              break;
          }

          status = response.Status;
          progress.Report(response.Progress);
          System.Diagnostics.Debug.WriteLine($"progress: {response.Progress}");
        }
      }
      catch (Exception ex)
      {
        _Logger.LogError(ex, "Error downloading file");
      }
      return status;
    }
    #endregion

    #region Process
    public async Task<string> ProcessAsync(string inputDataLocation, IProgress<ProcessingStatus> progress)
    {
      string outputDataDestination = string.Empty;

      try
      {
        using var call = _Client.Process(new ProcessingMetaData
        {
          DataLocation = inputDataLocation
        });

        await foreach (ProcessingInfo response in call.ResponseStream.ReadAllAsync())
        {
          switch (response.ResponseCase)
          {
            case ProcessingInfo.ResponseOneofCase.Status:
              progress.Report(response.Status);
              break;
            case ProcessingInfo.ResponseOneofCase.Destination:
              outputDataDestination = response.Destination.DataLocation;
              break;
            default:
              break;
          }
        }
      }
      catch (Exception ex)
      {
        _Logger.LogError(ex, "Processing error.");
      }
      return outputDataDestination;
    }
    #endregion

    private static GrpcChannel CreateChannel(string address)
    {
      var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions()
      {
        Credentials = ChannelCredentials.Insecure
      });
      return channel;
    }
  }
}

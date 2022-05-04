namespace ServiceLayer.SimulationKernel
{
  using DomainModel.SimulationKernel;
  using Generated;
  using Google.Protobuf;
  using Grpc.Core;
  using Grpc.Net.Client;
  using Microsoft.Extensions.Logging;
  using static Generated.FileTransfer;
  using Status = Generated.Status;

  internal class TransferDataService : ITransferDataService
  {
    private static readonly string _FileExtension = ".obj";
    private static readonly long _MaxFileSize = 1024 * 1024;

    private readonly ILogger<TransferDataService> _Logger;
    private readonly FileTransferClient _Client;

    public TransferDataService(ILogger<TransferDataService> logger)
    {
      _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
      var channel = CreateChannel("http://localhost:50051");
      _Client = new FileTransferClient(channel);
    }

    public async Task<Status> UploadAsync(FileData file, IProgress<uint> progress)
    {
      Status status = Status.Pending;
      try
      {
        using var transfer = _Client.Transfer();
        await SendMetadata(transfer, file.Name);

        int bytesRead = 0, totalRead = 0;

        var buffer = new byte[10 * 1024];//10KB buffer
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
        _Logger.LogWarning(ex, "Error loading file");
      }
      return status;
    }

    private static Task SendFileChunk(AsyncClientStreamingCall<FileTransferRequest, FileTransferResponse> transfer, int bytesRead, byte[] buffer)
    {
      return transfer.RequestStream.WriteAsync(new FileTransferRequest()
      {
        File = new File()
        {
          //ByteString.CopyFrom
          Chunk = UnsafeByteOperations.UnsafeWrap(buffer.AsMemory(0, bytesRead))
        }
      });
    }

    private static Task SendMetadata(AsyncClientStreamingCall<FileTransferRequest, FileTransferResponse> transfer, string name)
    {
      return transfer.RequestStream.WriteAsync(new FileTransferRequest()
      {
        Metadata = new FileMetaData()
        {
          Name = Path.GetFileNameWithoutExtension(name),
          Extension = _FileExtension
        }
      });
    }

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

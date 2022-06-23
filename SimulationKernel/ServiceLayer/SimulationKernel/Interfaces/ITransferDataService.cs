namespace ServiceLayer.SimulationKernel
{
  using DomainModel.SimulationKernel;
  using TransferStatus = Generated.TransferStatus;
  using ProcessingStatus = Generated.ProcessingStatus;

  public interface ITransferDataService
  {
    Task<TransferStatus> UploadAsync(FileData file, IProgress<uint> progress);
    Task<TransferStatus> DownloadAsync(string location, IProgress<uint> progress, Action<(byte[] bytes, string file)> onFileReceived);
    Task<string> ProcessAsync(string inputDataLocation, IProgress<ProcessingStatus> progress);
  }
}

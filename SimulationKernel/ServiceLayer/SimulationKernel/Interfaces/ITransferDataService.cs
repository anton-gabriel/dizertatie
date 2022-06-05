namespace ServiceLayer.SimulationKernel
{
  using DomainModel.SimulationKernel;
  using TransferStatus = Generated.TransferStatus;

  public interface ITransferDataService
  {
    Task<TransferStatus> UploadAsync(FileData file, IProgress<uint> progress);
    Task<TransferStatus> DownloadAsync(string location, IProgress<uint> progress);
  }
}

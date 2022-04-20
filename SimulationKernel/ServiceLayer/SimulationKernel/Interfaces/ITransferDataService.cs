namespace ServiceLayer.SimulationKernel
{
  using DomainModel.SimulationKernel;
  using Status = Generated.Status;

  public interface ITransferDataService
  {
    Task<Status> UploadAsync(FileData file, IProgress<uint> progress);
  }
}

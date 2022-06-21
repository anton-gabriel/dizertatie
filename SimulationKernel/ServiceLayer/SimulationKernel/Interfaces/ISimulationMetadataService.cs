namespace ServiceLayer.SimulationKernel
{
  using DomainModel.SimulationKernel;
  using FluentValidation.Results;
  using Microsoft.AspNetCore.Components.Forms;

  public interface ISimulationMetadataService : IService<SimulationMetadata>
  {
    Task<(ValidationResult, SimulationMetadata)> UploadUserDataAsync(
      string userName,
      IReadOnlyList<IBrowserFile> files,
      IProgress<uint> progress);

    Task<ProcessingStatus> RunUserProcessingAsync(
     string userName,
     SimulationMetadata simulation);

    IEnumerable<SimulationMetadata> GetUserProcessings(string userName, int limit);
  }
}

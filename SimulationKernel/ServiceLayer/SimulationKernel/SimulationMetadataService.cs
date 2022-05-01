namespace ServiceLayer.SimulationKernel
{
  using DataMapper.SimulationKernel.Repository;
  using DomainModel.SimulationKernel;
  using FluentValidation;
  using Microsoft.Extensions.Logging;

  internal sealed class SimulationMetadataService : Service<SimulationMetadata, ISimulationMetadataRepository>, ISimulationMetadataService
  {
    public SimulationMetadataService(ISimulationMetadataRepository repository, IValidator<SimulationMetadata> validator, ILogger<SimulationMetadataService> logger)
      : base(repository, validator, logger)
    {
    }
  }
}

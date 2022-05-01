namespace DataMapper.SimulationKernel.Repository
{
  using DataMapper.SimulationKernel.Context;
  using DomainModel.SimulationKernel;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// Class UserRepository. This class cannot be inherited.
  /// Implements the <see cref="DataMapper.SimulationKernel.Repository.Repository{DomainModel.SimulationKernel.SimulationMetadata}" />
  /// Implements the <see cref="DataMapper.SimulationKernel.Repository.ISimulationMetadataRepository" />
  /// </summary>
  /// <seealso cref=DataMapper.SimulationKernel.Repository.Repository{DomainModel.SimulationKernel.SimulationMetadata}" />
  /// <seealso cref=DataMapper.SimulationKernel.Repository.ISimulationMetadataRepository" />
  internal sealed class SimulationMetadataRepository : Repository<SimulationMetadata>, ISimulationMetadataRepository
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SimulationMetadataRepository"/> class.
    /// </summary>
    /// <param name="contextFactory">The context factory.</param>
    /// <param name="logger">The logger.</param>
    public SimulationMetadataRepository(IDbContextFactory<SimulationKernelContext> contextFactory, ILogger<SimulationMetadataRepository> logger)
      : base(contextFactory, logger)
    {
    }
  }
}

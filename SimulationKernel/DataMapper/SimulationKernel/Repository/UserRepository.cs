namespace DataMapper.SimulationKernel.Repository
{
  using DataMapper.SimulationKernel.Context;
  using DomainModel.SimulationKernel;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// Class UserRepository. This class cannot be inherited.
  /// Implements the <see cref="DataMapper.SimulationKernel.Repository.Repository{DomainModel.SimulationKernel.User}" />
  /// Implements the <see cref="DataMapper.SimulationKernel.Repository.IUserRepository" />
  /// </summary>
  /// <seealso cref=DataMapper.SimulationKernel.Repository.Repository{DomainModel.SimulationKernel.User}" />
  /// <seealso cref=DataMapper.SimulationKernel.Repository.IUserRepository" />
  internal sealed class UserRepository : Repository<User>, IUserRepository
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="contextFactory">The context factory.</param>
    /// <param name="logger">The logger.</param>
    public UserRepository(IDbContextFactory<SimulationKernelContext> contextFactory, ILogger<UserRepository> logger)
      : base(contextFactory, logger)
    {
    }
  }
}

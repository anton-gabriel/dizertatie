namespace DataMapper.SimulationKernel.Repository
{
  using DataMapper.SimulationKernel.Context;
  using DomainModel.SimulationKernel;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;

  internal sealed class UserRepository : Repository<User>, IUserRepository
  {
    public UserRepository(IDbContextFactory<SimulationKernelContext> contextFactory, ILogger<UserRepository> logger)
      : base(contextFactory, logger)
    {
    }
  }
}
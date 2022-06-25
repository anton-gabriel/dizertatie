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

    public int GetNumberOfProcessings(string userName)
    {
      try
      {
        using var context = this.ContextFactory.CreateDbContext();
        IQueryable<User> query = context.Set<User>();
        User user = query.SingleOrDefault(user => user.UserName == userName);
        return user != null ? user.Simulations.Count() : 0;
      }
      catch (Exception exception)
      {
        string message = string.Format(Resources.ExceptionOccurredMessage, nameof(SingleOrDefault), exception.Message);
        this.Logger.LogWarning(message);
        throw new RepositoryException(message, exception);
      }
    }
  }
}
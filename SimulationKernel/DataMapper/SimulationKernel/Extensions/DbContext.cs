namespace DataMapper.SimulationKernel.Extensions
{
  using DataMapper.SimulationKernel.Context;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.DependencyInjection;

  /// <summary>
  /// Class with database context extension methods.
  /// </summary>
  public static class DbContext
  {
    /// <summary>
    /// Adds the context.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentException">When <paramref name="connectionString"/> cannot be null or whitespace.</exception>
    public static IServiceCollection AddContext(this IServiceCollection services, string connectionString)
    {
      if (services is null)
      {
        throw new ArgumentNullException(nameof(services));
      }

      if (string.IsNullOrWhiteSpace(connectionString))
      {
        throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or whitespace", nameof(connectionString));
      }

      return services.AddDbContextFactory<SimulationKernelContext>(options =>
      {
        options
          .UseLazyLoadingProxies()
          .UseSqlServer(connectionString);
      });
    }

    /// <summary>
    /// Modifies the state.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <param name="entity">The entity.</param>
    /// <param name="state">The state.</param>
    public static void ModifyState<TEntity>(this Microsoft.EntityFrameworkCore.DbContext dbContext, TEntity entity, EntityState state) where TEntity : class
    {
      dbContext.Entry(entity).State = state;
    }

    /// <summary>
    /// Checks the state of the entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <param name="entity">The entity.</param>
    /// <param name="state">The state.</param>
    /// <returns><c>true</c> if state matches entity state, <c>false</c> otherwise.</returns>
    public static bool CheckEntityState<TEntity>(this Microsoft.EntityFrameworkCore.DbContext dbContext, TEntity entity, EntityState state) where TEntity : class
    {
      return dbContext.Entry(entity).State == state;
    }
  }
}

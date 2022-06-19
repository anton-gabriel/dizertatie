namespace DataMapper.SimulationKernel.Repository
{
  using DataMapper.SimulationKernel.Context;
  using DataMapper.SimulationKernel.Extensions;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Expressions;

  /// <summary>
  /// Represents the base class for each repository class.
  /// </summary>
  /// <typeparam name="T">The repository entity type.</typeparam>
  /// <seealso cref="Library.DataMapper.Repository.IRepository{T}" />
  /// <remarks>This is an abstract class. Repository classes should handle entity framework's exceptions
  /// (e.g. <see cref="DbUpdateException" />) so that client code doesn't have to depend on Entity Framework.</remarks>
  internal abstract class Repository<T> : IRepository<T> where T : class
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{T}" /> class.
    /// </summary>
    /// <param name="contextFactory">The context factory.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="contextFactory"/> is null.</exception>
    /// <exception cref="ArgumentNullException">When <paramref name="logger"/> is null.</exception>
    public Repository(IDbContextFactory<SimulationKernelContext> contextFactory, ILogger<Repository<T>> logger)
    {
      this.ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
      this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the context factory.
    /// </summary>
    /// <value>The context factory.</value>
    protected IDbContextFactory<SimulationKernelContext> ContextFactory { get; }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    /// <value>The logger.</value>
    protected ILogger<Repository<T>> Logger { get; }

    /// <summary>
    /// Removes the entity with the specified id.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="id"/> is null.</exception>
    /// <exception cref="Library.DataMapper.Repository.RepositoryException">When the operation cannot be executed.</exception>
    public virtual void Delete(object id)
    {
      if (id is null)
      {
        throw new ArgumentNullException(nameof(id));
      }

      this.Delete(this.Get(id));
    }

    /// <summary>
    /// Removes the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="entity"/> is null.</exception>
    /// <exception cref="Library.DataMapper.Repository.RepositoryException">When the operation cannot be executed.</exception>
    public virtual void Delete(T entity)
    {
      if (entity is null)
      {
        throw new ArgumentNullException(nameof(entity));
      }

      try
      {
        using var context = this.ContextFactory.CreateDbContext();
        var set = context.Set<T>();
        if (context.CheckEntityState(entity, EntityState.Detached))
        {
          set.Attach(entity);
        }

        set.Remove(entity);
        context.SaveChanges();
      }
      catch (Exception exception)
      {
        string message = string.Format(Resources.ExceptionOccurredMessage, nameof(Delete), exception.Message);
        this.Logger.LogWarning(message);
        throw new RepositoryException(message, exception);
      }
    }

    /// <summary>
    /// Removes the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="entities"/> is null.</exception>
    /// <exception cref="Library.DataMapper.Repository.RepositoryException">When the operation cannot be executed.</exception>
    public virtual void DeleteRange(IEnumerable<T> entities)
    {
      if (entities is null)
      {
        throw new ArgumentNullException(nameof(entities));
      }

      try
      {
        using var context = this.ContextFactory.CreateDbContext();
        var set = context.Set<T>();
        foreach (var entity in entities)
        {
          if (context.CheckEntityState(entity, EntityState.Detached))
          {
            set.Attach(entity);
          }
        }

        set.RemoveRange(entities);
        context.SaveChanges();
      }
      catch (Exception exception)
      {
        string message = string.Format(Resources.ExceptionOccurredMessage, nameof(DeleteRange), exception.Message);
        this.Logger.LogWarning(message);
        throw new RepositoryException(message, exception);
      }
    }

    /// <summary>
    /// Finds all entities that meet the specified conditions.
    /// </summary>
    /// <param name="filter">The filter expression for the search.</param>
    /// <param name="orderBy">The order by expression to order the results.</param>
    /// <param name="include">The relations which will be included for the entity.</param>
    /// <returns>The sequence of resulted entities.</returns>
    /// <exception cref="Library.DataMapper.Repository.RepositoryException">When the operation cannot be executed.</exception>
    public virtual IEnumerable<T> Find(
      Expression<Func<T, bool>> filter = null,
      Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
      params Expression<Func<T, object>>[] include)
    {
      try
      {
        using var context = this.ContextFactory.CreateDbContext();
        IQueryable<T> query = context.Set<T>();

        foreach (var relation in include)
        {
          query = query.Include(relation);
        }

        if (filter is not null)
        {
          query = query.Where(filter);
        }

        return orderBy is null ? query.ToList() : orderBy(query).ToList();
      }
      catch (Exception exception)
      {
        string message = string.Format(Resources.ExceptionOccurredMessage, nameof(Find), exception.Message);
        this.Logger.LogWarning(message);
        throw new RepositoryException(message, exception);
      }
    }

    /// <summary>
    /// Gets the entity with the specified id.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The entity with the given id.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="id"/> is null.</exception>
    /// <exception cref="Library.DataMapper.Repository.RepositoryException">When the operation cannot be executed.</exception>
    public virtual T Get(object id)
    {
      if (id is null)
      {
        throw new ArgumentNullException(nameof(id));
      }

      try
      {
        using var context = this.ContextFactory.CreateDbContext();
        var set = context.Set<T>();
        return set.Find(id);
      }
      catch (Exception exception)
      {
        string message = string.Format(Resources.ExceptionOccurredMessage, nameof(Get), exception.Message);
        this.Logger.LogWarning(message);
        throw new RepositoryException(message, exception);
      }
    }

    /// <summary>
    /// Insert the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="entity"/> is null.</exception>
    /// <exception cref="Library.DataMapper.Repository.RepositoryException">When the operation cannot be executed.</exception>
    public virtual void Insert(T entity)
    {
      if (entity is null)
      {
        throw new ArgumentNullException(nameof(entity));
      }

      try
      {
        using var context = this.ContextFactory.CreateDbContext();
        var set = context.Set<T>();
        set.Add(entity);
        context.SaveChanges();
      }
      catch (Exception exception)
      {
        string message = string.Format(Resources.ExceptionOccurredMessage, nameof(Insert), exception.Message);
        this.Logger.LogWarning(message);
        throw new RepositoryException(message, exception);
      }
    }

    /// <summary>
    /// Insert the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="entities"/> is null.</exception>
    /// <exception cref="Library.DataMapper.Repository.RepositoryException">When the operation cannot be executed.</exception>
    public virtual void InsertRange(IEnumerable<T> entities)
    {
      if (entities is null)
      {
        throw new ArgumentNullException(nameof(entities));
      }

      try
      {
        using var context = this.ContextFactory.CreateDbContext();
        var set = context.Set<T>();
        set.AddRange(entities);
        context.SaveChanges();
      }
      catch (Exception exception)
      {
        string message = string.Format(Resources.ExceptionOccurredMessage, nameof(InsertRange), exception.Message);
        this.Logger.LogWarning(message);
        throw new RepositoryException(message, exception);
      }
    }

    /// <summary>
    /// Finds the entity which meet the specified conditions.
    /// </summary>
    /// <param name="filter">The filter expression for the search.</param>
    /// <param name="include">The relations which will be included for the entity.</param>
    /// <returns>The found entity or null otherwise.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="filter"/> is null.</exception>
    /// <exception cref="Library.DataMapper.Repository.RepositoryException">When the operation cannot be executed.</exception>
    public virtual T SingleOrDefault(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] include)
    {
      if (filter is null)
      {
        throw new ArgumentNullException(nameof(filter));
      }

      try
      {
        using var context = this.ContextFactory.CreateDbContext();
        IQueryable<T> query = context.Set<T>();

        foreach (var relation in include)
        {
          query = query.Include(relation);
        }

        return query.SingleOrDefault(filter);
      }
      catch (Exception exception)
      {
        string message = string.Format(Resources.ExceptionOccurredMessage, nameof(SingleOrDefault), exception.Message);
        this.Logger.LogWarning(message);
        throw new RepositoryException(message, exception);
      }
    }

    /// <summary>
    /// Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="entity"/> is null.</exception>
    /// <exception cref="Library.DataMapper.Repository.RepositoryException">When the operation cannot be executed.</exception>
    public virtual void Update(T entity, Action<T> updateMethod = null)
    {
      if (entity is null)
      {
        throw new ArgumentNullException(nameof(entity));
      }

      try
      {
        using var context = this.ContextFactory.CreateDbContext();
        var set = context.Set<T>();
        set.Attach(entity);
        updateMethod?.Invoke(entity);
        context.ModifyState(entity, EntityState.Modified);
        context.SaveChanges();
      }
      catch (Exception exception)
      {
        string message = string.Format(Resources.ExceptionOccurredMessage, nameof(Update), exception.Message);
        this.Logger.LogWarning(message);
        throw new RepositoryException(message, exception);
      }
    }
  }
}

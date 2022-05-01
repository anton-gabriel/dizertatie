namespace ServiceLayer.SimulationKernel
{
  using DataMapper.SimulationKernel.Repository;
  using FluentValidation;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// Represents the base class for services types.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TRepository">The type of the repository.</typeparam>
  /// <remarks>This is an abstract class.</remarks>
  internal abstract class Service<TEntity, TRepository> :
    IService<TEntity>
    where TEntity : class
    where TRepository : IRepository<TEntity>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Service{TEntity, TRepository}" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="System.ArgumentNullException">When <paramref name="repository"/> is null.</exception>
    /// <exception cref="System.ArgumentNullException">When <paramref name="validator"/> is null.</exception>
    /// <exception cref="System.ArgumentNullException">When <paramref name="logger"/> is null.</exception>
    public Service(
      TRepository repository,
      IValidator<TEntity> validator,
      ILogger<Service<TEntity, TRepository>> logger)
    {
      this._Repository = repository ?? throw new System.ArgumentNullException(nameof(repository));
      this._Validator = validator ?? throw new System.ArgumentNullException(nameof(validator));
      this._Logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    /// <value>The logger.</value>
    protected ILogger<Service<TEntity, TRepository>> _Logger { get; }

    /// <summary>
    /// Gets the repository.
    /// </summary>
    /// <value>The repository.</value>
    protected TRepository _Repository { get; }

    /// <summary>
    /// Gets the validator.
    /// </summary>
    /// <value>The validator.</value>
    protected IValidator<TEntity> _Validator { get; }

    /// <summary>
    /// Adds the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <exception cref="System.ArgumentNullException">When <paramref name="entity"/> is null.</exception>
    /// <exception cref="ValidationException">When <paramref name="entity"/> is not valid.</exception>
    public virtual void Add(TEntity entity)
    {
      if (entity is null)
      {
        throw new System.ArgumentNullException(nameof(entity));
      }

      this._Validator.ValidateAndThrow(entity);
      this._Repository.Insert(entity);
      this._Logger.LogInformation(string.Format(Resources.EntityAdded, typeof(TEntity)));
    }

    /// <summary>
    /// Gets the entity by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The entity <typeparamref name="T" /></returns>
    /// <exception cref="System.ArgumentNullException">When <paramref name="id"/> is null.</exception>
    public virtual TEntity GetByID(object id)
    {
      if (id is null)
      {
        throw new System.ArgumentNullException(nameof(id));
      }

      return this._Repository.Get(id);
    }

    /// <summary>
    /// Removes the specified entity.
    /// </summary>
    /// <param name="id">The entity id.</param>
    /// <exception cref="System.ArgumentNullException">When <paramref name="entity"/> is null.</exception>
    public virtual void Remove(object id)
    {
      if (id is null)
      {
        throw new System.ArgumentNullException(nameof(id));
      }

      this._Repository.Delete(id);
      this._Logger.LogInformation(string.Format(Resources.EntityRemoved, typeof(TEntity)));
    }

    /// <summary>
    /// Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <exception cref="System.ArgumentNullException">When <paramref name="entity"/> is null.</exception>
    /// <exception cref="ValidationException">When <paramref name="entity"/> is not valid.</exception>
    public virtual void Update(TEntity entity)
    {
      if (entity is null)
      {
        throw new System.ArgumentNullException(nameof(entity));
      }

      this._Validator.ValidateAndThrow(entity);
      this._Repository.Update(entity);
      this._Logger.LogInformation(string.Format(Resources.EntityUpdated, typeof(TEntity)));
    }
  }
}

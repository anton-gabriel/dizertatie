namespace ServiceLayer.SimulationKernel
{
  /// <summary>
  /// Represents the service contract.
  /// </summary>
  /// <typeparam name="T">The entity type of the service.</typeparam>
  public interface IService<T> where T : class
  {
    /// <summary>
    /// Adds the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Add(T entity);

    /// <summary>
    /// Gets the entity by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The entity <typeparamref name="T"/></returns>
    T GetByID(object id);

    /// <summary>
    /// Removes the specified entity.
    /// </summary>
    /// <param name="id">The entity id.</param>
    void Remove(object id);

    /// <summary>
    /// Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Update(T entity);
  }
}

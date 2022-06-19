namespace DataMapper.SimulationKernel.Repository
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Expressions;

  /// <summary>
  /// Represents the repository contract with CRUD operations.
  /// </summary>
  /// <typeparam name="T">The entity type of the repository.</typeparam>
  public interface IRepository<T> where T : class
  {
    /// <summary>
    /// Removes the entity with the specified id.
    /// </summary>
    /// <param name="id">The identifier.</param>
    void Delete(object id);

    /// <summary>
    /// Removes the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Delete(T entity);

    /// <summary>
    /// Removes the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    void DeleteRange(IEnumerable<T> entities);

    /// <summary>
    /// Finds all entities that meet the specified conditions.
    /// </summary>
    /// <param name="filter">The filter expression for the search.</param>
    /// <param name="orderBy">The order by expression to order the results.</param>
    /// <param name="include">The relations which will be included for the entity.</param>
    /// <returns>The sequence of resulted entities.</returns>
    IEnumerable<T> Find(
      Expression<Func<T, bool>> filter = null,
      Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
      params Expression<Func<T, object>>[] include);

    /// <summary>
    /// Gets the entity with the specified id.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The entity with the given id.</returns>
    T Get(object id);

    /// <summary>
    /// Insert the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Insert(T entity);

    /// <summary>
    /// Insert the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    void InsertRange(IEnumerable<T> entities);

    /// <summary>
    /// Finds the entity which meet the specified conditions.
    /// </summary>
    /// <param name="filter">The filter expression for the search.</param>
    /// <param name="include">The relations which will be included for the entity.</param>
    /// <returns>The found entity or null otherwise.</returns>
    T SingleOrDefault(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] include);

    /// <summary>
    /// Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Update(T entity, Action<T> updateMethod = null);
  }
}

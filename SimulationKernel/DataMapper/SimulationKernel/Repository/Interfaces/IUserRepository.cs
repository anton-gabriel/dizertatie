namespace DataMapper.SimulationKernel.Repository
{
  using DomainModel.SimulationKernel;

  public interface IUserRepository : IRepository<User>
  {
    int GetNumberOfProcessings(string userName);
  }
}

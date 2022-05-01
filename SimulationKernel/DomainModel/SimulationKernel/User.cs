namespace DomainModel.SimulationKernel
{
  using Microsoft.AspNetCore.Identity;

  public class User : IdentityUser<int>
  {
    public virtual ICollection<SimulationMetadata> Domains { get; set; }
  }
}

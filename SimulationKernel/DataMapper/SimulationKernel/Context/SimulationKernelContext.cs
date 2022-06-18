namespace DataMapper.SimulationKernel.Context
{
  using DomainModel.SimulationKernel;
  using Microsoft.AspNetCore.Identity;
  using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Configuration;

  /// <summary>
  /// Class SimulationKernelContext.
  /// Implements the <see cref="Microsoft.EntityFrameworkCore.DbContext" />
  /// </summary>
  /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
  internal class SimulationKernelContext : IdentityDbContext<User, IdentityRole<int>, int>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SimulationKernelContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public SimulationKernelContext(DbContextOptions<SimulationKernelContext> options)
      : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the users.
    /// </summary>
    /// <value>The users.</value>
    public virtual DbSet<SimulationMetadata> SimulationDataItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true)
        .Build();

      var connectionString = config.GetConnectionString("sqlServer");
      optionsBuilder
        .UseSqlServer(connectionString, options => options.MigrationsAssembly("DataMapper.SimulationKernel"))
        .UseLazyLoadingProxies();
    }
  }
}

namespace DomainModel.SimulationKernel
{
  public static class AppOpptions
  {
    public static long MaxFileSize { get; } = 10 * 1024 * 1024;//10 MB
    public static string AllowedExtension { get; } = ".obj";
  }
}

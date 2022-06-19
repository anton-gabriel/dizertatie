namespace DomainModel.SimulationKernel
{
  public record FileData(Stream ReadStream, string Name, long Size, string Destination) : IDisposable
  {
    private bool _DisposedValue;

    protected virtual void Dispose(bool disposing)
    {
      if (!_DisposedValue)
      {
        if (disposing)
        {
          ReadStream?.Dispose();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        _DisposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~FileData()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
  }
}
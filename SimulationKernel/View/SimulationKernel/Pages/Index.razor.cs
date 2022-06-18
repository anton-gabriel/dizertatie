namespace SimulationKernel.Pages
{
  using DomainModel.SimulationKernel;
  using Generated;
  using Microsoft.AspNetCore.Components;
  using Microsoft.AspNetCore.Components.Forms;
  using Microsoft.JSInterop;
  using ServiceLayer.SimulationKernel;
  using SimulationKernel.Data;
  using System;
  using System.Linq;
  using System.Threading.Tasks;

  public partial class Index : ComponentBase, IAsyncDisposable
  {
    private static readonly string _AllowedExtension = ".obj";
    private ElementReference _CanvasHostReference;
    private IJSObjectReference? _JSModule;
    private uint? _ProgressPercent;
    private string? _UploadMessage;
    private IReadOnlyList<IBrowserFile> _UserFiles = new List<IBrowserFile>();
    private bool _Uploaded = false;
    private readonly List<ObjectData> _DataFrames = new();
    private static readonly long _MaxFileSize = 10 * 1024 * 1024;//10 MB

    [Inject]
    private ILogger<Index> Logger { get; set; } = default!;
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject]
    private ITransferDataService TransferDataService { get; set; } = default!;
    [Inject]
    private ProcessedDataService ProcessedDataService { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      if (firstRender)
      {
        _JSModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/js/scene.js");
        if (_JSModule != null && _CanvasHostReference.Context != null)
        {
          await _JSModule.InvokeVoidAsync("renderScene", _CanvasHostReference);
        }
      }

      await base.OnAfterRenderAsync(firstRender);
    }

    private async Task Process()
    {
      string filePath = Path.GetFullPath("out_blender");
      string[] files = Directory.GetFiles(filePath, "*.obj").OrderBy(name => name).ToArray();
      foreach (string file in files)
      {
        ObjectData data = ProcessedDataService.ReadObjFile(file);
        if (_JSModule != null)
        {
          await _JSModule.InvokeVoidAsync("updateScene", data);
          await Task.Delay(100);
        }
      }
    }

    private async Task ResetCamera()
    {
      if (_JSModule != null)
      {
        await _JSModule.InvokeVoidAsync("resetCamera");
      }
    }

    private void LoadFiles(InputFileChangeEventArgs e)
    {
      int maximumFileCount = 50;
      try
      {
        _UserFiles = e.GetMultipleFiles(maximumFileCount);
        _Uploaded = false;
        _ProgressPercent = null;
      }
      catch (InvalidOperationException exception)
      {
        Logger.LogWarning(exception, $"Cannot load more than {maximumFileCount} files.");
      }
    }

    private async Task UploadFilesAsync()
    {
      if (_UserFiles.Any())
      {
        _DataFrames.Clear();
        for (int index = 0; index < _UserFiles.Count; ++index)
        {
          try
          {
            var file = _UserFiles[index];

            var fileInfo = new FileInfo(file.Name);
            bool validFormat = fileInfo.Extension.Equals(_AllowedExtension, StringComparison.OrdinalIgnoreCase);

            if (validFormat)
            {
              double weight = (double)index / _UserFiles.Count;
              TransferStatus status = await UploadFile(file, weight);
              if (status == TransferStatus.Succeded)
              {
                //Load the scene only if upload was successful
                _Uploaded = true;
                ObjectData data = await ProcessedDataService.ReadObjFileAsync(file.OpenReadStream(_MaxFileSize));
                _DataFrames.Add(data);
              }
              else
              {
                _UploadMessage = "File upload failed";
              }
            }
          }
          catch (IOException exception)
          {
            _UploadMessage = $"Max file size is {_MaxFileSize} bytes";
            Logger.LogError(exception, _UploadMessage);
          }
          catch (Exception exception)
          {
            _UploadMessage = "File upload failed";
            Logger.LogError(exception, _UploadMessage);
          }
        }
        _ProgressPercent = null;
      }
    }

    private async Task<TransferStatus> UploadFile(IBrowserFile file, double fileWeight)
    {
      using var fileData = new FileData(file.OpenReadStream(_MaxFileSize), file.Name, file.Size);
      TransferStatus result = await TransferDataService.UploadAsync(fileData, new Progress<uint>((percent) =>
      {
        _ProgressPercent = (uint)(percent * fileWeight);
        StateHasChanged();
      }));
      return result;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
      if (_JSModule is not null)
      {
        await _JSModule.DisposeAsync();
      }
      GC.SuppressFinalize(this);
    }
  }
}
namespace SimulationKernel.Pages
{
  using Microsoft.AspNetCore.Components;
  using Microsoft.AspNetCore.Components.Forms;
  using Microsoft.JSInterop;
  using System;
  using System.Linq;
  using System.Threading.Tasks;

  public partial class Index
  {
    ElementReference CanvasHostReference;
    private double[][]? _ProcessedData;
    private IJSObjectReference? _Module;
    private uint? _ProgressPercent;
    private IBrowserFile? _UserFile;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      if (firstRender)
      {
        _Module = await _JS.InvokeAsync<IJSObjectReference>("import", "/js/scene.js");
        if (_Module != null)
        {
          await _Module.InvokeVoidAsync("renderScene", CanvasHostReference);
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
        _ProcessedData = await _ProcessedDataService.ReadObjFile(file);
        if (_Module != null)
        {
          await _Module.InvokeVoidAsync("updateScene", (object)_ProcessedData);
          await Task.Delay(100);
        }
      }
    }

    private async Task ResetCamera()
    {
      if (_Module != null)
      {
        await _Module.InvokeVoidAsync("resetCamera");
      }
    }

    private void LoadFile(InputFileChangeEventArgs e)
    {
      _UserFile = e.File;
      _Uploaded = false;
      _ProgressPercent = null;
    }

    private async Task UploadFileAsync()
    {
      if (_UserFile != null)
      {
        await _TransferDataService.UploadAsync(_UserFile, new Progress<uint>((percent) =>
        {
          _ProgressPercent = percent;
          StateHasChanged();
        })).ContinueWith(t => _Uploaded = true);
        await Task.Delay(300).ContinueWith(t => _ProgressPercent = null);
      }
    }


    async ValueTask IAsyncDisposable.DisposeAsync()
    {
      if (_Module is not null)
      {
        await _Module.DisposeAsync();
      }
      GC.SuppressFinalize(this);
    }
  }
}
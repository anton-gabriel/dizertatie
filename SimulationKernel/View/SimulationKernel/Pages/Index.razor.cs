namespace SimulationKernel.Pages
{
  using DomainModel.SimulationKernel;
  using Generated;
  using Microsoft.AspNetCore.Components;
  using Microsoft.AspNetCore.Components.Forms;
  using Microsoft.JSInterop;
  using System;
  using System.Linq;
  using System.Threading.Tasks;

  public partial class Index
  {
    ElementReference CanvasHostReference;
    private IJSObjectReference? _Module;
    private uint? _ProgressPercent;
    private string? _UploadMessage;
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
        double[][]? data = await _ProcessedDataService.ReadObjFile(file);
        if (_Module != null)
        {
          await _Module.InvokeVoidAsync("updateScene", (object)data);
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
        Status status = await UploadFile(_UserFile);
        if (status == Status.Succeded)
        {
          //Load the scene only if upload was successful
          _Uploaded = true;
          await LoadScene(_UserFile);
        }
        else
        {
          _UploadMessage = "File upload failed";
        }
        _ProgressPercent = null;
      }
    }

    private async Task<Status> UploadFile(IBrowserFile file)
    {
      var fileData = new FileData(file.OpenReadStream(), file.Name, file.Size);
      Status result = await _TransferDataService.UploadAsync(fileData, new Progress<uint>((percent) =>
      {
        _ProgressPercent = percent;
        StateHasChanged();
      }));
      return result;
    }

    private async Task LoadScene(IBrowserFile file)
    {
      double[][] data = await _ProcessedDataService.ReadObjFileAsync(file.OpenReadStream());
      if (_Module != null)
      {
        await _Module.InvokeVoidAsync("updateScene", (object)data);
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
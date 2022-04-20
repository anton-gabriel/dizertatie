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
        //To do
        FileData fileData = await UploadFile(_UserFile);
        //Load the scene only if upload was successful
        FileData fileData1 = new FileData(_UserFile.OpenReadStream(), _UserFile.Name, _UserFile.Size);
        await LoadScene(fileData1);
        //await Task.Delay(300).ContinueWith(t => _ProgressPercent = null);
      }
    }

    private async Task<FileData> UploadFile(IBrowserFile file)
    {
      var fileData = new FileData(file.OpenReadStream(), file.Name, file.Size);
      await _TransferDataService.UploadAsync(fileData, new Progress<uint>((percent) =>
      {
        _ProgressPercent = percent;
        StateHasChanged();
      })).ContinueWith(t =>
      {
        Status status = t.Result;
        if (status == Status.Succeded)
        {
          _Uploaded = true;
        }
        else
        {
          _UploadMessage = "Upload failed";
        }
      });
      return fileData;
    }

    private async Task LoadScene(FileData file)
    {
      double[][] data = await _ProcessedDataService.ReadObjFileAsync(file.ReadStream);
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
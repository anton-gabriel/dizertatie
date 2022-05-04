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
    ElementReference CanvasHostReference;
    private IJSObjectReference? _Module;
    private uint? _ProgressPercent;
    private string? _UploadMessage;
    private IEnumerable<IBrowserFile> _UserFiles = Enumerable.Empty<IBrowserFile>();
    private bool _Uploaded = false;

    [Inject]
    private IJSRuntime JS { get; set; } = default!;
    [Inject]
    private ITransferDataService TransferDataService { get; set; } = default!;
    [Inject]
    private ProcessedDataService ProcessedDataService { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      if (firstRender)
      {
        _Module = await JS.InvokeAsync<IJSObjectReference>("import", "/js/scene.js");
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
        double[][]? data = await ProcessedDataService.ReadObjFile(file);
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
      _UserFiles = e.GetMultipleFiles(maximumFileCount: 2);
      _Uploaded = false;
      _ProgressPercent = null;
    }

    private async Task UploadFileAsync()
    {
      if (_UserFiles.Any())
      {
        Status status = await UploadFile(_UserFiles.First());
        if (status == Status.Succeded)
        {
          //Load the scene only if upload was successful
          _Uploaded = true;
          await LoadSceneAsync(_UserFiles);
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
      Status result = await TransferDataService.UploadAsync(fileData, new Progress<uint>((percent) =>
      {
        _ProgressPercent = percent;
        StateHasChanged();
      }));
      return result;
    }

    private async Task LoadSceneAsync(IEnumerable<IBrowserFile> files)
    {
      List<Task<ObjectData>> list = files
        .AsParallel()
        .AsOrdered()
        .Select(file =>
          ProcessedDataService.ReadObjFileAsync(file.OpenReadStream()))
        .ToList();

      var results = await Task.WhenAll(list);

      if (_Module != null && results.Any())
      {
        await _Module.InvokeVoidAsync("updateScene", results.First());
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
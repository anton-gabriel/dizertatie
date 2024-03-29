namespace SimulationKernel.Pages
{
  using DomainModel.SimulationKernel;
  using FluentValidation.Results;
  using Microsoft.AspNetCore.Components;
  using Microsoft.AspNetCore.Components.Authorization;
  using Microsoft.AspNetCore.Components.Forms;
  using Microsoft.JSInterop;
  using ServiceLayer.SimulationKernel;
  using System;
  using System.Linq;
  using System.Threading.Tasks;

  public partial class Index : ComponentBase, IAsyncDisposable
  {
    private ElementReference _CanvasHostReference;
    private IJSObjectReference? _JSModule;
    private uint? _ProgressPercent;
    private string? _UploadMessage;
    private IReadOnlyList<IBrowserFile> _UserFiles = new List<IBrowserFile>();
    private bool _Uploaded = false;
    private bool _Running = false;
    private readonly List<ObjectData> _DataFrames = new();
    private SimulationMetadata? _SimulationMetadata;

    [Inject]
    private ILogger<Index> Logger { get; set; } = default!;
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject]
    private ISimulationMetadataService SimulationService { get; set; } = default!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;


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
      var authState = await AuthenticationStateTask;
      var user = authState.User;
      if (user.Identity != null && user.Identity.IsAuthenticated)
      {
        var userName = user.Identity.Name;
        if (_SimulationMetadata != null)
        {
          _Running = true;
          await SimulationService.RunUserProcessingAsync(userName, _SimulationMetadata);
          _Running = false;
        }
        else
        {
          Logger.LogWarning("No simulation data available");
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
        var authState = await AuthenticationStateTask;
        var user = authState.User;
        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
          var userName = user.Identity.Name;

          (ValidationResult result, SimulationMetadata simulation) = await SimulationService.UploadUserDataAsync(userName, _UserFiles, new Progress<uint>((percent) =>
          {
            _ProgressPercent = percent;
            StateHasChanged();
          }));
          if (result.IsValid)
          {
            _SimulationMetadata = simulation;
            _Uploaded = true;
            _UploadMessage = "Uploaded successfully";
            //foreach (var file in _UserFiles)
            //{
            //  ObjectData data = await ProcessedDataService.ReadObjFileAsync(file.OpenReadStream(AppOpptions.MaxFileSize));
            //  _DataFrames.Add(data);
            //}
          }

          //Select first frame
          if (_JSModule != null && _UserFiles.Any())
          {
            await _JSModule.InvokeVoidAsync("updateSceneFromObjectFile", 0);
          }
          _ProgressPercent = null;
        }
      }
    }

    private async Task UpdateFrame(ChangeEventArgs e)
    {
      int frame = Convert.ToInt32(e.Value);

      if (_JSModule != null && frame < _UserFiles.Count)
      {
        await _JSModule.InvokeVoidAsync("updateSceneFromObjectFile", frame);
      }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
      try
      {
        if (_JSModule is not null)
        {
          await _JSModule.DisposeAsync();
        }
        GC.SuppressFinalize(this);
      }
      catch (JSDisconnectedException)
      {
        System.Diagnostics.Debug.WriteLine("JavaScript runtime disconnected (SignalR disconnected).");
      }
    }
  }
}
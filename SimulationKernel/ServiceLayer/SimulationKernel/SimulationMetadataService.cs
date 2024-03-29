﻿namespace ServiceLayer.SimulationKernel
{
  using DataMapper.SimulationKernel.Repository;
  using DomainModel.SimulationKernel;
  using FluentValidation;
  using FluentValidation.Results;
  using Microsoft.AspNetCore.Components.Forms;
  using Microsoft.Extensions.Logging;

  internal sealed class SimulationMetadataService : Service<SimulationMetadata, ISimulationMetadataRepository>, ISimulationMetadataService
  {
    private readonly IUserRepository _UserRepository;
    private readonly ITransferDataService _TransferDataService;

    public SimulationMetadataService(
      IUserRepository userRepository,
      ITransferDataService transferDataService,
      ISimulationMetadataRepository repository,
      IValidator<SimulationMetadata> validator,
      ILogger<SimulationMetadataService> logger)
      : base(repository, validator, logger)
    {
      _UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
      _TransferDataService = transferDataService ?? throw new ArgumentNullException(nameof(transferDataService));
    }

    public async Task<(ValidationResult, SimulationMetadata)> UploadUserDataAsync(
      string userName,
      IReadOnlyList<IBrowserFile> files,
      IProgress<uint> progress)
    {
      SimulationMetadata simulation = null;
      var result = new ValidationResult();
      string message = string.Empty;
      var user = _UserRepository.SingleOrDefault(user => user.UserName == userName);

      if (user != null)
      {
        string processingName = $"Processing_{DateTime.Now:MMddyyyyssffffff}";
        string destination = Path.Combine(AppOpptions.DestinationDataDirectoryName, userName, processingName);

        for (int index = 0; index < files.Count; ++index)
        {
          try
          {
            var file = files[index];

            var fileInfo = new FileInfo(file.Name);
            bool validFormat = fileInfo.Extension.Equals(AppOpptions.AllowedExtension, StringComparison.OrdinalIgnoreCase);

            if (validFormat)
            {
              double weight = (double)index / files.Count;
              Generated.TransferStatus status = await UploadFile(file, weight, destination, progress);
              if (status != Generated.TransferStatus.Succeded)
              {
                message = $"Cannot upload '{file.Name}' file.";
                _Logger.LogError(message);
              }
            }
          }
          catch (IOException exception)
          {
            message = $"Max file size is {AppOpptions.MaxFileSize} bytes";
            _Logger.LogError(exception, message);
          }
          catch (Exception exception)
          {
            message = "File upload failed";
            _Logger.LogError(exception, message);
          }
        }

        simulation = new SimulationMetadata()
        {
          CreationDate = DateTime.Now,
          Name = processingName,
          Status = ProcessingStatus.NotStarted,
          InputDataLocation = destination,
        };

        result = _Validator.Validate(simulation);
        if (result.IsValid)
        {
          _UserRepository.Update(user, user => user.Simulations.Add(simulation));
        }
      }

      return (result, simulation);
    }

    public async Task<ProcessingStatus> RunUserProcessingAsync(
      string userName,
      SimulationMetadata simulation)
    {
      ProcessingStatus result = ProcessingStatus.NotStarted;
      var user = _UserRepository.SingleOrDefault(
       user => user.UserName == userName
      );

      if (user != null && simulation.Status == ProcessingStatus.NotStarted)
      {
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        var resultsOutputDestination = await _TransferDataService.ProcessAsync(simulation.InputDataLocation, new Progress<Generated.ProcessingStatus>(progress =>
        {
          result = progress switch
          {
            Generated.ProcessingStatus.NotStarted => ProcessingStatus.NotStarted,
            Generated.ProcessingStatus.Processing => ProcessingStatus.Processing,
            Generated.ProcessingStatus.Processed => ProcessingStatus.Completed,
            _ => throw new NotImplementedException(),
          };
        }));

        stopwatch.Stop();
        //Update simulation results location
        _Repository.Update(simulation,
          simulation =>
          {
            simulation.OutputDataLocation = resultsOutputDestination;
            simulation.Status = ProcessingStatus.Completed;
            simulation.Duration = stopwatch.Elapsed;
          });
      }

      return result;
    }

    public IEnumerable<SimulationMetadata> GetUserProcessings(string userName, int pageNumber, int pageSize)
    {
      var user = _UserRepository.SingleOrDefault(
        user => user.UserName == userName,
        user => user.Simulations
          .OrderByDescending(s => s.CreationDate)
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
     );

      return user != null ? user.Simulations : Enumerable.Empty<SimulationMetadata>();
    }

    public int GetUserProcessings(string userName)
    {
      int result = _UserRepository.GetNumberOfProcessings(userName);
      return result;
    }


    private async Task<Generated.TransferStatus> UploadFile(IBrowserFile file, double fileWeight, string destination, IProgress<uint> progress)
    {
      using var fileData = new FileData(file.OpenReadStream(AppOpptions.MaxFileSize), file.Name, file.Size, destination);
      Generated.TransferStatus result = await _TransferDataService.UploadAsync(fileData, new Progress<uint>((percent) =>
      {
        //Report weighted progress (relative to all files)
        progress.Report((uint)(percent * fileWeight));
      }));
      return result;
    }
  }
}

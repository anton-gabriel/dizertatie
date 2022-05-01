﻿namespace DomainModel.SimulationKernel
{
  public class SimulationDataItem
  {
    public int Id { get; private set; }
    public string Name { get; set; }
    public ProcessingStatus Status { get; set; }
    public DateTime CreationDate { get; set; }
    public TimeSpan? Duration { get; set; }

    public static SimulationDataItem Empty => new()
    {
      Id = -1,
      Name = string.Empty,
      Status = ProcessingStatus.NotStarted,
      CreationDate = DateTime.MinValue,
      Duration = TimeSpan.FromMilliseconds(0.0),
    };
  }
}

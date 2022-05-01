namespace ServiceLayer.SimulationKernel.Validators
{
  using DomainModel.SimulationKernel;
  using FluentValidation;
  
  internal sealed class SimulationMetadataValidator : AbstractValidator<SimulationMetadata>
  {
    public SimulationMetadataValidator()
    {
      RuleFor(metadata => metadata.Name)
        .NotEmpty()
        .MinimumLength(1)
        .MaximumLength(512)
        .Matches(@"^[a-zA-Z0-9_\-\.]+$")
        .WithMessage(Resources.AlphanumericRequired);

      RuleFor(metadata => metadata.CreationDate)
        .LessThanOrEqualTo(DateTime.Now)
        .GreaterThanOrEqualTo(new DateTime(2022, 1, 1));

      RuleFor(metadata => metadata.DataLocation)
        .NotEmpty();
    }
  }
}

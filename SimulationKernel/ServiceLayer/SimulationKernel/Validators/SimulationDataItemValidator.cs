namespace ServiceLayer.SimulationKernel.Validators
{
  using DomainModel.SimulationKernel;
  using FluentValidation;
  
  internal sealed class SimulationDataItemValidator : AbstractValidator<SimulationDataItem>
  {
    public SimulationDataItemValidator()
    {
      RuleFor(item => item.Name)
        .NotEmpty()
        .MinimumLength(1)
        .MaximumLength(512)
        .Matches(@"^[a-zA-Z0-9_\-\.]+$")
        .WithMessage(Resources.AlphanumericRequired);

      RuleFor(item => item.CreationDate)
        .LessThanOrEqualTo(DateTime.Now)
        .GreaterThanOrEqualTo(new DateTime(2022, 1, 1));
    }
  }
}

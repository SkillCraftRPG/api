using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Lineages.Models;

public record UpdateLineagePayload
{

  public string? Name { get; set; }
  public Optional<string>? Summary { get; set; }
  public Optional<string>? Content { get; set; }

  public LineageLanguagesPayload? Languages { get; set; }
  public LineageNamesModel? Names { get; set; }
  public LineageSpeedsModel? Speeds { get; set; }
  public LineageSizeModel? Size { get; set; }
  public LineageWeightModel? Weight { get; set; }
  public LineageAgeModel? Age { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateLineagePayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
      When(x => !string.IsNullOrWhiteSpace(x.Content?.Value), () => RuleFor(x => x.Content!.Value!).Content());

      When(x => x.Languages is not null, () => RuleFor(x => x.Languages!).SetValidator(new LineageLanguagesValidator()));
      When(x => x.Names is not null, () => RuleFor(x => x.Names!).SetValidator(new LineageNamesValidator()));
      When(x => x.Speeds is not null, () => RuleFor(x => x.Speeds!).SetValidator(new LineageSpeedsValidator()));
      When(x => x.Size is not null, () => RuleFor(x => x.Size!).SetValidator(new LineageSizeValidator()));
      When(x => x.Weight is not null, () => RuleFor(x => x.Weight!).SetValidator(new LineageWeightValidator()));
      When(x => x.Age is not null, () => RuleFor(x => x.Age!).SetValidator(new LineageAgeValidator()));
    }
  }
}

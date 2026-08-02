using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Lineages.Models;

public record LineageWeightModel
{
  public string? Malnutrition { get; set; }
  public string? Skinny { get; set; }
  public string? Normal { get; set; }
  public string? Overweight { get; set; }
  public string? Obese { get; set; }
}

internal class LineageWeightValidator : AbstractValidator<LineageWeightModel>
{
  public LineageWeightValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Malnutrition), () => RuleFor(x => x.Malnutrition!).Roll());
    When(x => !string.IsNullOrWhiteSpace(x.Skinny), () => RuleFor(x => x.Skinny!).Roll());
    When(x => !string.IsNullOrWhiteSpace(x.Normal), () => RuleFor(x => x.Normal!).Roll());
    When(x => !string.IsNullOrWhiteSpace(x.Overweight), () => RuleFor(x => x.Overweight!).Roll());
    When(x => !string.IsNullOrWhiteSpace(x.Obese), () => RuleFor(x => x.Obese!).Roll());
  }
}

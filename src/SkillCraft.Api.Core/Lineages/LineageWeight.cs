using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Lineages;

public interface ILineageWeight
{
  string? Malnutrition { get; }
  string? Skinny { get; }
  string? Normal { get; }
  string? Overweight { get; }
  string? Obese { get; }
}

[method: JsonConstructor]
public record LineageWeight(string? Malnutrition, string? Skinny, string? Normal, string? Overweight, string? Obese) : ILineageWeight
{
  public LineageWeight(Lineage lineage) : this(lineage.Malnutrition, lineage.Skinny, lineage.NormalWeight, lineage.NormalWeight, lineage.Obese)
  {
  }
}

internal class LineageWeightValidator : AbstractValidator<ILineageWeight>
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

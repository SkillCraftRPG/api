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

public record LineageWeight : ILineageWeight
{
  public string? Malnutrition { get; }
  public string? Skinny { get; }
  public string? Normal { get; }
  public string? Overweight { get; }
  public string? Obese { get; }

  public LineageWeight()
  {
  }

  [JsonConstructor]
  public LineageWeight(string? malnutrition, string? skinny, string? normal, string? overweight, string? obese)
  {
    Malnutrition = malnutrition;
    Skinny = skinny;
    Normal = normal;
    Overweight = overweight;
    Obese = obese;
  }

  public LineageWeight(Lineage lineage) : this(lineage.Malnutrition, lineage.Skinny, lineage.NormalWeight, lineage.Overweight, lineage.Obese)
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

using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Lineages;

public interface ILineageSize
{
  SizeCategory Category { get; }
  string? Height { get; }
}

[method: JsonConstructor]
public record LineageSize(SizeCategory Category, string? Height) : ILineageSize
{
  public LineageSize(Lineage lineage) : this(lineage.SizeCategory, lineage.HeightRoll)
  {
  }
}

internal class LineageSizeValidator : AbstractValidator<ILineageSize>
{
  public LineageSizeValidator()
  {
    RuleFor(x => x.Category).IsInEnum();
    When(x => !string.IsNullOrWhiteSpace(x.Height), () => RuleFor(x => x.Height!).Roll());
  }
}

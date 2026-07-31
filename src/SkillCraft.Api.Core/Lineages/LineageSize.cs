using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Lineages;

public interface ILineageSize
{
  SizeCategory Category { get; }
  string? Height { get; }
}

public record LineageSize : ILineageSize
{
  public SizeCategory Category { get; }
  public string? Height { get; }

  public LineageSize()
  {
  }

  [JsonConstructor]
  public LineageSize(SizeCategory category, string? height)
  {
    Category = category;
    Height = height;
  }

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

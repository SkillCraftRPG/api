using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Lineages.Models;

public record LineageSizeModel
{
  public SizeCategory Category { get; set; }
  public string? Height { get; set; }
}

internal class LineageSizeValidator : AbstractValidator<LineageSizeModel>
{
  public LineageSizeValidator()
  {
    RuleFor(x => x.Category).IsInEnum();
    When(x => !string.IsNullOrWhiteSpace(x.Height), () => RuleFor(x => x.Height!).Roll());
  }
}

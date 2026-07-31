using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Lineages.Models;

public record LineageLanguagesPayload
{
  public List<Guid> Ids { get; set; } = [];
  public int Extra { get; set; }
  public string? Content { get; set; }
}

internal class LineageLanguagesValidator : AbstractValidator<LineageLanguagesPayload>
{
  public LineageLanguagesValidator()
  {
    RuleFor(x => x.Extra).GreaterThanOrEqualTo(0);
    When(x => !string.IsNullOrWhiteSpace(x.Content), () => RuleFor(x => x.Content!).Content());
  }
}

using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Castes.Models;

public record CreateOrReplaceCastePayload
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public Skill? Skill { get; set; }
  public string? WealthRoll { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceCastePayload>
  {
    public Validator()
    {
      RuleFor(x => x.Name).Name();
      When(x => !string.IsNullOrWhiteSpace(x.Summary), () => RuleFor(x => x.Summary!).Summary());
      When(x => !string.IsNullOrWhiteSpace(x.HtmlContent), () => RuleFor(x => x.HtmlContent!).HtmlContent());

      When(x => x.Skill.HasValue, () => RuleFor(x => x.Skill!.Value).IsInEnum());
      When(x => !string.IsNullOrWhiteSpace(x.WealthRoll), () => RuleFor(x => x.WealthRoll!).Roll());
    }
  }
}

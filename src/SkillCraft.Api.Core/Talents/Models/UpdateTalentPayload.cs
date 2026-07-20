using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Talents.Models;

public record UpdateTalentPayload
{
  public string? Name { get; set; }
  public Optional<string>? Summary { get; set; }
  public Optional<string>? HtmlContent { get; set; }

  public bool? AllowMultiplePurchases { get; set; }
  public Optional<Skill?>? Skill { get; set; }
  public Optional<Guid?>? RequiredTalentId { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateTalentPayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
      When(x => !string.IsNullOrWhiteSpace(x.HtmlContent?.Value), () => RuleFor(x => x.HtmlContent!.Value!).HtmlContent());

      When(x => x.Skill?.Value is not null, () => RuleFor(x => x.Skill!.Value!.Value).IsInEnum());
    }
  }
}

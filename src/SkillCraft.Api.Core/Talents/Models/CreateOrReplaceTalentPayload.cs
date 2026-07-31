using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Talents.Models;

public record CreateOrReplaceTalentPayload
{
  public int Tier { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Content { get; set; }

  public bool AllowMultiplePurchases { get; set; }
  public Skill? Skill { get; set; }
  public Guid? RequiredTalentId { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceTalentPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Tier).TalentTier();

      RuleFor(x => x.Name).Name();
      When(x => !string.IsNullOrWhiteSpace(x.Summary), () => RuleFor(x => x.Summary!).Summary());
      When(x => !string.IsNullOrWhiteSpace(x.Content), () => RuleFor(x => x.Content!).Content());

      When(x => x.Skill.HasValue, () => RuleFor(x => x.Skill!.Value).IsInEnum());
    }
  }
}

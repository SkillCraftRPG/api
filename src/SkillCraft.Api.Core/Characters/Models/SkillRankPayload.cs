using FluentValidation;

namespace SkillCraft.Api.Core.Characters.Models;

public record SkillRankPayload
{
  public Skill Skill { get; set; }
  public int Rank { get; set; }
}

internal class SkillRankValidator : AbstractValidator<SkillRankPayload>
{
  public SkillRankValidator()
  {
    RuleFor(x => x.Skill).IsInEnum();
    RuleFor(x => x.Rank).InclusiveBetween(0, 2);
  }
}

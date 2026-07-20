using FluentValidation;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Educations.Models;

public record CreateOrReplaceEducationPayload
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public Skill? Skill { get; set; }
  public int? WealthMultiplier { get; set; }
  public FeatureModel? Feature { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceEducationPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Name).Name();
      When(x => !string.IsNullOrWhiteSpace(x.Summary), () => RuleFor(x => x.Summary!).Summary());
      When(x => !string.IsNullOrWhiteSpace(x.HtmlContent), () => RuleFor(x => x.HtmlContent!).HtmlContent());

      When(x => x.Skill.HasValue, () => RuleFor(x => x.Skill!.Value).IsInEnum());
      When(x => x.WealthMultiplier.HasValue, () => RuleFor(x => x.WealthMultiplier!.Value).InclusiveBetween(1, 999));
      When(x => x.Feature is not null, () => RuleFor(x => x.Feature!).SetValidator(new FeatureValidator()));
    }
  }
}

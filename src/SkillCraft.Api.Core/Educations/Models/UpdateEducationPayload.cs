using FluentValidation;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Educations.Models;

public record UpdateEducationPayload
{
  public string? Name { get; set; }
  public Optional<string>? Summary { get; set; }
  public Optional<string>? HtmlContent { get; set; }

  public Optional<Skill?>? Skill { get; set; }
  public Optional<int?>? WealthMultiplier { get; set; }
  public Optional<FeatureModel>? Feature { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateEducationPayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
      When(x => !string.IsNullOrWhiteSpace(x.HtmlContent?.Value), () => RuleFor(x => x.HtmlContent!.Value!).HtmlContent());

      When(x => x.Skill?.Value is not null, () => RuleFor(x => x.Skill!.Value!.Value).IsInEnum());
      When(x => x.WealthMultiplier?.Value is not null, () => RuleFor(x => x.WealthMultiplier!.Value!.Value).InclusiveBetween(1, 999));
      When(x => x.Feature?.Value is not null, () => RuleFor(x => x.Feature!.Value!).SetValidator(new FeatureValidator()));
    }
  }
}

using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Characters.Models;

public record UpdateCharacterPayload
{
  public string? Name { get; set; }
  public Optional<DominantHand?>? DominantHand { get; set; }

  public CharacterAppearance? Appearance { get; set; }
  public Optional<Alignment?>? Alignment { get; set; }
  public CharacterPersonality? Personality { get; set; }
  public Optional<string>? Background { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateCharacterPayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => x.DominantHand?.Value is not null, () => RuleFor(x => x.DominantHand!.Value!.Value).IsInEnum());

      When(x => x.Appearance is not null, () => RuleFor(x => x.Appearance!).SetValidator(new CharacterAppearanceValidator()));
      When(x => x.Alignment?.Value is not null, () => RuleFor(x => x.Alignment!.Value!.Value).IsInEnum());
      When(x => x.Personality is not null, () => RuleFor(x => x.Personality!).SetValidator(new CharacterPersonalityValidator()));
      When(x => !string.IsNullOrWhiteSpace(x.Background?.Value), () => RuleFor(x => x.Background!.Value!).Background());
    }
  }
}

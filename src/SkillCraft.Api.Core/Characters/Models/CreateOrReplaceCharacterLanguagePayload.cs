using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Characters.Models;

public record CreateOrReplaceCharacterLanguagePayload
{
  public CharacterLanguageSource Source { get; set; }
  public string? Target { get; set; }
  public string? Notes { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceCharacterLanguagePayload>
  {
    public Validator()
    {
      RuleFor(x => x.Source).IsInEnum();
      When(x => x.Source == CharacterLanguageSource.Custom, () => RuleFor(x => x.Target!).Name());
      When(x => x.Source == CharacterLanguageSource.Extra, () => RuleFor(x => x.Target!).Empty());
      When(x => x.Source == CharacterLanguageSource.Customization || x.Source == CharacterLanguageSource.Talent,
        () => RuleFor(x => x.Target!).SetValidator(new UuidValidator<CreateOrReplaceCharacterLanguagePayload>()));
      When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
    }
  }
}

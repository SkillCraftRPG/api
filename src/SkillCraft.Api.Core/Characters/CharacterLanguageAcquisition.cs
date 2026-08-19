using FluentValidation;
using Logitar;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Characters;

public record CharacterLanguageAcquisition
{
  public CharacterLanguageSource Source { get; }
  public string? Target { get; }
  public Notes? Notes { get; }

  public CharacterLanguageAcquisition(CharacterLanguageSource source, string? target = null, Notes? notes = null)
  {
    Source = source;
    Target = target?.CleanTrim();
    Notes = notes;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<CharacterLanguageAcquisition>
  {
    public Validator()
    {
      RuleFor(x => x.Source).IsInEnum();
      When(x => x.Source == CharacterLanguageSource.Custom, () => RuleFor(x => x.Target!).Name());
      When(x => x.Source == CharacterLanguageSource.Extra, () => RuleFor(x => x.Target).Null());
      When(x => x.Source == CharacterLanguageSource.Customization || x.Source == CharacterLanguageSource.Talent,
        () => RuleFor(x => x.Target!).SetValidator(new UuidValidator<CharacterLanguageAcquisition>()));
    }
  }
}

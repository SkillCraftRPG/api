using FluentValidation;

namespace SkillCraft.Api.Core.Characters;

public interface ICharacterPersonality
{
  string? Traits { get; }
  string? Ideals { get; }
  string? Flaws { get; }
}

public record CharacterPersonality : ICharacterPersonality
{
  public string? Traits { get; }
  public string? Ideals { get; }
  public string? Flaws { get; }

  public CharacterPersonality()
  {
  }

  [JsonConstructor]
  public CharacterPersonality(string? traits, string? ideals, string? flaws)
  {
    Traits = traits;
    Ideals = ideals;
    Flaws = flaws;
  }

  public CharacterPersonality(ICharacterPersonality personality) : this(personality.Traits, personality.Ideals, personality.Flaws)
  {
  }
}

internal class CharacterPersonalityValidator : AbstractValidator<ICharacterPersonality>
{
  public CharacterPersonalityValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Traits), () => RuleFor(x => x.Traits).NotEmpty());
    When(x => !string.IsNullOrWhiteSpace(x.Ideals), () => RuleFor(x => x.Ideals).NotEmpty());
    When(x => !string.IsNullOrWhiteSpace(x.Flaws), () => RuleFor(x => x.Flaws).NotEmpty());
  }
}

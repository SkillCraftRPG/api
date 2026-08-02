using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Languages;

public class TypicalSpeakers
{
  public string Value { get; }

  public TypicalSpeakers(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static TypicalSpeakers? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override bool Equals(object? obj) => obj is TypicalSpeakers speakers && speakers.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;

  private class Validator : AbstractValidator<TypicalSpeakers>
  {
    public Validator()
    {
      RuleFor(x => x.Value).TypicalSpeakers();
    }
  }
}

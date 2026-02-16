using FluentValidation;

namespace SkillCraft.Api.Core.Languages;

public record TypicalSpeakers
{
  public string Value { get; }
  public long Size => Value.Length;

  public TypicalSpeakers(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static TypicalSpeakers? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<TypicalSpeakers>
  {
    public Validator()
    {
      RuleFor(x => x.Value).TypicalSpeakers();
    }
  }
}

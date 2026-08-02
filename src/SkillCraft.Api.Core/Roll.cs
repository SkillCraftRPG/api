using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core;

public class Roll
{
  public const int MaximumLength = 10;

  public string Value { get; }

  public Roll(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Roll? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override bool Equals(object? obj) => obj is Roll roll && roll.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;

  private class Validator : AbstractValidator<Roll>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Roll();
    }
  }
}

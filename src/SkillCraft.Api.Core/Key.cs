using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core;

public class Key
{
  public const int MaximumLength = 100;

  public string Value { get; }

  public Key(string value)
  {
    Value = SlugHelper.Format(value);
    new Validator().ValidateAndThrow(this);
  }

  public static Key? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override bool Equals(object? obj) => obj is Key key && key.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;

  private class Validator : AbstractValidator<Key>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Key();
    }
  }
}

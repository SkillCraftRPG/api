using FluentValidation;

namespace SkillCraft.Api.Core;

public record Roll
{
  public const int MaximumLength = 10;

  public string Value { get; }
  public long Size => Value.Length;

  public Roll(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Roll? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Roll>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Roll();
    }
  }
}

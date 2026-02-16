using FluentValidation;

namespace SkillCraft.Api.Core;

public record Roll
{
  public const int MaximumLength = 10;
  public const string Pattern = "^(?:([1-9]\\d{0,2})\\+)?((?:100|[1-9]\\d?))[dD]((?:100|[1-9]\\d?))$";

  public string Value { get; }
  public long Size => Value.Length;

  public Roll(string value)
  {
    Value = value.Trim().ToLowerInvariant();
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

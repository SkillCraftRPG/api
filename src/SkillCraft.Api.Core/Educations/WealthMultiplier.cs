using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Educations;

public class WealthMultiplier
{
  public const int MinimumValue = 1;
  public const int MaximumValue = 999;

  public int Value { get; }

  public WealthMultiplier(int value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public static WealthMultiplier? TryCreate(int? value) => value.HasValue ? new(value.Value) : null;

  public override bool Equals(object? obj) => obj is WealthMultiplier multiplier && multiplier.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<WealthMultiplier>
  {
    public Validator()
    {
      RuleFor(x => x.Value).WealthMultiplier();
    }
  }
}

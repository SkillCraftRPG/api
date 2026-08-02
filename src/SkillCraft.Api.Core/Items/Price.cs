using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Items;

public class Price
{
  public double Value { get; }

  public Price(double value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public static Price? TryCreate(double? value) => value.HasValue ? new(value.Value) : null;

  public override bool Equals(object? obj) => obj is Price price && price.Value.Equals(Value);
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<Price>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Price();
    }
  }
}

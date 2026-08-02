using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Items;

public class Weight
{
  public double Value { get; }

  public Weight(double value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public static Weight? TryCreate(double? value) => value.HasValue ? new(value.Value) : null;

  public override bool Equals(object? obj) => obj is Weight weight && weight.Value.Equals(Value);
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<Weight>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Weight();
    }
  }
}

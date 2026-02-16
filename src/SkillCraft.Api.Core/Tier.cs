using FluentValidation;

namespace SkillCraft.Api.Core;

public record Tier
{
  public const int Minimum = 0;
  public const int Maximum = 3;

  public int Value { get; }

  public Tier(int value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public static Tier? TryCreate(int? value) => value.HasValue ? new(value.Value) : null;

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<Tier>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Tier();
    }
  }
}

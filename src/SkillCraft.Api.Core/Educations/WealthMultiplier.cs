using FluentValidation;

namespace SkillCraft.Api.Core.Educations;

public record WealthMultiplier
{
  public int Value { get; }

  public WealthMultiplier(int value)
  {
    Value = value;
  }

  public static WealthMultiplier? TryCreate(int? value) => value.HasValue ? new(value.Value) : null;

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<WealthMultiplier>
  {
    public Validator()
    {
      RuleFor(x => x.Value).WealthMultiplier();
    }
  }
}

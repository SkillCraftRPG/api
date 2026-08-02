using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Talents;

public class TalentTier
{
  public const int MinimumValue = 0;
  public const int MaximumValue = 3;

  public int Value { get; }

  public TalentTier(int value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public override bool Equals(object? obj) => obj is TalentTier tier && tier.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<TalentTier>
  {
    public Validator()
    {
      RuleFor(x => x.Value).TalentTier();
    }
  }
}

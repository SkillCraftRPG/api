using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Characters;

public interface ICharacterTalentDiscount
{
  CharacterTalentDiscountSource Source { get; }
  string Target { get; }
  int Amount { get; }
}

public record CharacterTalentDiscount : ICharacterTalentDiscount
{
  public CharacterTalentDiscountSource Source { get; }
  public string Target { get; }
  public int Amount { get; }

  [JsonConstructor]
  public CharacterTalentDiscount(CharacterTalentDiscountSource source, string target, int amount)
  {
    Source = source;
    Target = target;
    Amount = amount;
    new CharacterTalentDiscountValidator().ValidateAndThrow(this);
  }

  public CharacterTalentDiscount(ICharacterTalentDiscount discount) : this(discount.Source, discount.Target, discount.Amount)
  {
  }
}

internal class CharacterTalentDiscountValidator : AbstractValidator<ICharacterTalentDiscount>
{
  public CharacterTalentDiscountValidator()
  {
    RuleFor(x => x.Source).IsInEnum();
    When(x => x.Source == CharacterTalentDiscountSource.Custom, () => RuleFor(x => x.Target).Name())
      .Otherwise(() => RuleFor(x => x.Target).SetValidator(new UuidValidator<ICharacterTalentDiscount>()));
    RuleFor(x => x.Amount).InclusiveBetween(1, 5);
  }
}

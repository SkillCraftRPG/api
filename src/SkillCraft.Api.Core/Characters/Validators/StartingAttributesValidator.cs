using FluentValidation;
using SkillCraft.Api.Contracts.Characters;

namespace SkillCraft.Api.Core.Characters.Validators;

internal class StartingAttributesValidator : AbstractValidator<IStartingAttributes>
{
  public StartingAttributesValidator()
  {
    RuleFor(x => x.Dexterity).InclusiveBetween(StartingAttributes.MinimumValue, StartingAttributes.MaximumValue);
    RuleFor(x => x.Health).InclusiveBetween(StartingAttributes.MinimumValue, StartingAttributes.MaximumValue);
    RuleFor(x => x.Intellect).InclusiveBetween(StartingAttributes.MinimumValue, StartingAttributes.MaximumValue);
    RuleFor(x => x.Senses).InclusiveBetween(StartingAttributes.MinimumValue, StartingAttributes.MaximumValue);
    RuleFor(x => x.Vigor).InclusiveBetween(StartingAttributes.MinimumValue, StartingAttributes.MaximumValue);

    RuleFor(x => x).Must(HaveValidTotal)
      .WithErrorCode(nameof(StartingAttributesValidator))
      .WithMessage("Starting attributes sum must equal 0.");
  }

  private static bool HaveValidTotal(IStartingAttributes attributes)
  {
    return new int[] { attributes.Dexterity, attributes.Health, attributes.Intellect, attributes.Senses, attributes.Vigor }.Sum() == 0;
  }
}

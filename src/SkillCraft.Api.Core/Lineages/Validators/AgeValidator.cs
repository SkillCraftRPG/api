using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Core.Lineages.Validators;

internal class AgeValidator : AbstractValidator<IAge>
{
  public AgeValidator()
  {
    RuleFor(x => x).Must(HaveValidThresholds)
      .WithErrorCode(nameof(AgeValidator))
      .WithMessage("'{PropertyName}' must be either all null or strictly increasing and greater than 0.");
  }

  private static bool HaveValidThresholds(IAge age)
  {
    int?[] thresholds = [age.Teenager, age.Adult, age.Mature, age.Venerable];
    if (thresholds.All(x => x is null))
    {
      return true;
    }
    else if (thresholds.Any(x => x is null))
    {
      return false;
    }

    int[] values = thresholds.Select(x => x!.Value).ToArray();
    if (values.Any(x => x <= 0))
    {
      return false;
    }

    return values[0] < values[1] && values[1] < values[2] && values[2] < values[3];
  }
}

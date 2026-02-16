using FluentValidation;

namespace SkillCraft.Api.Core;

internal static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> Description<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty();
  }

  public static IRuleBuilderOptions<T, string> Name<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Core.Name.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Roll<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Core.Roll.MaximumLength); // TODO(fpion): RegExp
  }

  public static IRuleBuilderOptions<T, string> Summary<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Core.Summary.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> TypicalSpeakers<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty();
  }

  public static IRuleBuilderOptions<T, int> WealthMultiplier<T>(this IRuleBuilder<T, int> ruleBuilder)
  {
    return ruleBuilder.GreaterThan(0);
  }
}

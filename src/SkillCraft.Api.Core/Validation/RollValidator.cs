using FluentValidation;
using FluentValidation.Validators;

namespace SkillCraft.Api.Core.Validation;

internal class RollValidator<T> : IPropertyValidator<T, string>
{
  public string Name { get; } = "RollValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return "'{PropertyName}' must be a valid roll.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    value = value.ToLowerInvariant();

    string[] parts = value.Split('+');
    if (parts.Length > 2)
    {
      return false;
    }
    if (parts.Length == 2)
    {
      if (!int.TryParse(parts.First(), out int @base) || @base < 1 || @base > 999)
      {
        return false;
      }
    }

    string[] roll = parts.Last().Split('d');
    if (roll.Length != 2)
    {
      return false;
    }
    if (!int.TryParse(roll.First(), out int dice) || dice < 1 || dice > 99)
    {
      return false;
    }
    if (!int.TryParse(roll.Last(), out int die) || die < 1 || die > 999)
    {
      return false;
    }

    return true;
  }
}

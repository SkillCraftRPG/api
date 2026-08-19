using FluentValidation;
using FluentValidation.Validators;

namespace SkillCraft.Api.Core.Validation;

internal class UuidValidator<T> : IPropertyValidator<T, string>
{
  public string Name { get; } = "UuidValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return "'{PropertyName}' must be a valid universally unique identifier (UUID).";
  }

  public bool IsValid(ValidationContext<T> context, string value) => Guid.TryParse(value, out _);
}

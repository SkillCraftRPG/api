using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Customizations.Models;

public record CreateOrReplaceCustomizationPayload
{
  public CustomizationKind Kind { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceCustomizationPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Kind).IsInEnum();

      RuleFor(x => x.Name).Name();
      When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
    }
  }
}

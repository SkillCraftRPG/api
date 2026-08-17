using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Characters.Models;

public record CreateOrReplaceCharacterModifierPayload
{
  public CharacterModifierKind Kind { get; set; }
  public string Target { get; set; } = string.Empty;

  public int Value { get; set; }

  public string? Name { get; set; }
  public string? Notes { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceCharacterModifierPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Kind).IsInEnum();
      RuleFor(x => x.Target).Must(HaveAValidTarget)
        .WithErrorCode("TargetValidator")
        .WithMessage(p => $"'{{PropertyName}}' must be a valid target for the kind '{p.Kind}'.");

      RuleFor(x => x.Value).NotEmpty();

      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
    }

    private static bool HaveAValidTarget(CreateOrReplaceCharacterModifierPayload payload, string target) => payload.Kind switch
    {
      CharacterModifierKind.Attribute => Enum.TryParse(target, ignoreCase: true, out GameAttribute attribute) && Enum.IsDefined(attribute),
      CharacterModifierKind.Skill => Enum.TryParse(target, ignoreCase: true, out Skill skill) && Enum.IsDefined(skill),
      CharacterModifierKind.Speed => Enum.TryParse(target, ignoreCase: true, out Speed speed) && Enum.IsDefined(speed),
      CharacterModifierKind.Statistic => Enum.TryParse(target, ignoreCase: true, out Statistic statistic) && Enum.IsDefined(statistic),
      _ => true,
    };
  }
}

using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterTalentPayload
{
  public string? Qualifier { get; set; }
  public string? Notes { get; set; }
  public List<CharacterTalentDiscountModel> Discounts { get; set; } = [];
}

internal class CharacterTalentValidator : AbstractValidator<CharacterTalentPayload>
{
  public CharacterTalentValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Qualifier), () => RuleFor(x => x.Qualifier!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
    RuleForEach(x => x.Discounts).SetValidator(new CharacterTalentDiscountValidator());
  }
}

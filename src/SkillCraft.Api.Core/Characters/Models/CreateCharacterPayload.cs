using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Characters.Models;

public record CreateCharacterPayload
{
  public Guid LineageId { get; set; }
  public List<Guid> LanguageIds { get; set; } = [];

  public string Name { get; set; } = string.Empty;
  public DominantHand? DominantHand { get; set; }
  public List<Guid> CustomizationIds { get; set; } = [];

  public Guid CasteId { get; set; }
  public Guid EducationId { get; set; }

  public List<AddCharacterTalentPayload> Talents { get; set; } = [];

  public StartingAttributesModel Attributes { get; set; } = new();

  public List<SkillRankPayload> Skills { get; set; } = [];

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateCharacterPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Name).Name();
      RuleFor(x => x.DominantHand).IsInEnum();

      RuleForEach(x => x.Talents).SetValidator(new CharacterTalentValidator());

      RuleFor(x => x.Attributes).SetValidator(new StartingAttributesValidator());

      RuleForEach(x => x.Skills).SetValidator(new SkillRankValidator());
    }
  }
}

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

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateCharacterPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Name).Name();
      RuleFor(x => x.DominantHand).IsInEnum();
    }
  }
}

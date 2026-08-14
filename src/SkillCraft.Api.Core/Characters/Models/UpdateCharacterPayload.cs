using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Characters.Models;

public record UpdateCharacterPayload
{
  public string? Name { get; set; }
  public Optional<DominantHand?>? DominantHand { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateCharacterPayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => x.DominantHand?.Value is not null, () => RuleFor(x => x.DominantHand!.Value!.Value).IsInEnum());
    }
  }
}

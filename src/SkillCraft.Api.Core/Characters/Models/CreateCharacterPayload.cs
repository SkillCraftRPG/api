using FluentValidation;

namespace SkillCraft.Api.Core.Characters.Models;

public record CreateCharacterPayload
{
  public Guid LineageId { get; set; }
  public List<Guid> LanguageIds { get; set; } = [];

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateCharacterPayload>
  {
    public Validator()
    {
    }
  }
}

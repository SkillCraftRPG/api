using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Worlds.Models;

public record CreateOrReplaceWorldPayload
{
  public string Key { get; set; } = string.Empty;
  public string? Name { get; set; }
  public string? Content { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceWorldPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Key).Key();
      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Content), () => RuleFor(x => x.Content!).Content());
    }
  }
}

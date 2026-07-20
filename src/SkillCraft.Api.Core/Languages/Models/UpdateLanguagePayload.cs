using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Languages.Models;

public record UpdateLanguagePayload
{
  public string? Name { get; set; }
  public Optional<string>? Description { get; set; }

  public Optional<Guid?>? ScriptId { get; set; }
  public Optional<string>? TypicalSpeakers { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateLanguagePayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

      When(x => !string.IsNullOrWhiteSpace(x.TypicalSpeakers?.Value), () => RuleFor(x => x.TypicalSpeakers!.Value!).TypicalSpeakers());
    }
  }
}

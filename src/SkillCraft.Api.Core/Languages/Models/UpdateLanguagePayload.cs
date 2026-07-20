using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Languages.Models;

public record UpdateLanguagePayload
{
  public string? Name { get; set; }
  public Optional<string>? Summary { get; set; }
  public Optional<string>? HtmlContent { get; set; }

  public Optional<Guid?>? ScriptId { get; set; }
  public Optional<string>? TypicalSpeakers { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateLanguagePayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
      When(x => !string.IsNullOrWhiteSpace(x.HtmlContent?.Value), () => RuleFor(x => x.HtmlContent!.Value!).HtmlContent());

      When(x => !string.IsNullOrWhiteSpace(x.TypicalSpeakers?.Value), () => RuleFor(x => x.TypicalSpeakers!.Value!).TypicalSpeakers());
    }
  }
}

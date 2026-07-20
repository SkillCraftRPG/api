using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Languages.Models;

public record CreateOrReplaceLanguagePayload
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public Guid? ScriptId { get; set; }
  public string? TypicalSpeakers { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceLanguagePayload>
  {
    public Validator()
    {
      RuleFor(x => x.Name).Name();
      When(x => !string.IsNullOrWhiteSpace(x.Summary), () => RuleFor(x => x.Summary!).Summary());
      When(x => !string.IsNullOrWhiteSpace(x.HtmlContent), () => RuleFor(x => x.HtmlContent!).HtmlContent());

      When(x => !string.IsNullOrWhiteSpace(x.TypicalSpeakers), () => RuleFor(x => x.TypicalSpeakers!).TypicalSpeakers());
    }
  }
}

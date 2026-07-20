using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Scripts.Models;

public record UpdateScriptPayload
{
  public string? Name { get; set; }
  public Optional<string>? Summary { get; set; }
  public Optional<string>? HtmlContent { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateScriptPayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
      When(x => !string.IsNullOrWhiteSpace(x.HtmlContent?.Value), () => RuleFor(x => x.HtmlContent!.Value!).HtmlContent());
    }
  }
}

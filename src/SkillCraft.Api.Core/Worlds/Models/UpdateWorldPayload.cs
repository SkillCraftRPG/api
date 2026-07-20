using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Worlds.Models;

public record UpdateWorldPayload
{
  public string? Key { get; set; }
  public Optional<string>? Name { get; set; }
  public Optional<string>? HtmlContent { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateWorldPayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.Key), () => RuleFor(x => x.Key!).Slug());
      When(x => !string.IsNullOrWhiteSpace(x.Name?.Value), () => RuleFor(x => x.Name!.Value!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.HtmlContent?.Value), () => RuleFor(x => x.HtmlContent!.Value!).HtmlContent());
    }
  }
}

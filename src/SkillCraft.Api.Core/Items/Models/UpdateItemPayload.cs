using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Items.Models;

public record UpdateItemPayload
{
  public string? Name { get; set; }
  public Optional<string>? Summary { get; set; }
  public Optional<string>? Content { get; set; }

  public Optional<double?>? Price { get; set; }
  public Optional<double?>? Weight { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateItemPayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
      When(x => !string.IsNullOrWhiteSpace(x.Content?.Value), () => RuleFor(x => x.Content!.Value!).HtmlContent());

      When(x => x.Price?.Value is not null, () => RuleFor(x => x.Price!.Value!.Value).GreaterThan(0));
      When(x => x.Weight?.Value is not null, () => RuleFor(x => x.Weight!.Value!.Value).GreaterThan(0));
    }
  }
}

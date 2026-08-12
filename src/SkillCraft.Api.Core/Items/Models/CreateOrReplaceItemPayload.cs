using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Items.Models;

public record CreateOrReplaceItemPayload
{
  public ItemCategory Category { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Content { get; set; }

  public int? Price { get; set; }
  public int? Weight { get; set; }

  public ItemChargesPayload? Charges { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceItemPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Category).IsInEnum();

      RuleFor(x => x.Name).Name();
      When(x => !string.IsNullOrWhiteSpace(x.Summary), () => RuleFor(x => x.Summary!).Summary());
      When(x => !string.IsNullOrWhiteSpace(x.Content), () => RuleFor(x => x.Content!).Content());

      When(x => x.Price.HasValue, () => RuleFor(x => x.Price!.Value).Price());
      When(x => x.Weight.HasValue, () => RuleFor(x => x.Weight!.Value).Weight());

      When(x => x.Charges is not null, () => RuleFor(x => x.Charges!).SetValidator(new ItemChargesValidator()));
    }
  }
}

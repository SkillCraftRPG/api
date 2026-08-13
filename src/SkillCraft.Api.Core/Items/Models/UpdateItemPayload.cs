using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Items.Models;

public record UpdateItemPayload
{
  public string? Name { get; set; }
  public Optional<string>? Summary { get; set; }
  public Optional<string>? Content { get; set; }

  public Optional<int?>? Price { get; set; }
  public Optional<int?>? Weight { get; set; }

  public Optional<ItemRarity?>? Rarity { get; set; }
  public Optional<ItemChargesPayload>? Charges { get; set; }
  public Optional<MagicItemModel>? Magic { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateItemPayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
      When(x => !string.IsNullOrWhiteSpace(x.Content?.Value), () => RuleFor(x => x.Content!.Value!).Content());

      When(x => x.Price?.Value is not null, () => RuleFor(x => x.Price!.Value!.Value).Price());
      When(x => x.Weight?.Value is not null, () => RuleFor(x => x.Weight!.Value!.Value).Weight());

      When(x => x.Rarity?.Value is not null, () => RuleFor(x => x.Rarity!.Value!.Value).IsInEnum());
      When(x => x.Charges?.Value is not null, () => RuleFor(x => x.Charges!.Value!).SetValidator(new ItemChargesValidator()));
      When(x => x.Magic?.Value is not null, () => RuleFor(x => x.Magic!.Value!).SetValidator(new MagicItemValidator()));
    }
  }
}

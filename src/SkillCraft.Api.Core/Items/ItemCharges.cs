using FluentValidation;
using SkillCraft.Api.Core.Items.Models;

namespace SkillCraft.Api.Core.Items;

public record ItemCharges
{
  public int Maximum { get; }
  public DepletionBehavior DepletionBehavior { get; }
  public ItemId? ReplacementId { get; }

  [JsonConstructor]
  public ItemCharges(int maximum, DepletionBehavior depletionBehavior, ItemId? replacementId)
  {
    Maximum = maximum;
    DepletionBehavior = depletionBehavior;
    ReplacementId = replacementId;
  }

  public ItemCharges(int maximum, DepletionBehavior depletionBehavior, Item? replacement = null)
    : this(maximum, depletionBehavior, replacement?.Id)
  {
  }

  private class Validator : AbstractValidator<ItemCharges>
  {
    public Validator()
    {
      RuleFor(x => x.Maximum).GreaterThan(0);
      RuleFor(x => x.DepletionBehavior).IsInEnum();
      When(x => x.DepletionBehavior == DepletionBehavior.Replace, () => RuleFor(x => x.ReplacementId).NotNull())
        .Otherwise(() => RuleFor(x => x.ReplacementId).Null());
    }
  }
}

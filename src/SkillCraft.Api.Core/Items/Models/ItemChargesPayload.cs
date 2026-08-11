using FluentValidation;

namespace SkillCraft.Api.Core.Items.Models;

public record ItemChargesPayload
{
  public int Maximum { get; set; }
  public DepletionBehavior DepletionBehavior { get; set; }
  public Guid? ReplacementId { get; set; }
}

internal class ItemChargesValidator : AbstractValidator<ItemChargesPayload>
{
  public ItemChargesValidator()
  {
    RuleFor(x => x.Maximum).GreaterThan(0);
    RuleFor(x => x.DepletionBehavior).IsInEnum();
    When(x => x.DepletionBehavior == DepletionBehavior.Replace, () => RuleFor(x => x.ReplacementId).NotNull())
      .Otherwise(() => RuleFor(x => x.ReplacementId).Null());
  }
}

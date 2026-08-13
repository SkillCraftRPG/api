using FluentValidation;

namespace SkillCraft.Api.Core.Items.Models;

public record MagicItemModel
{
  public AttunementModel? Attunement { get; set; }
}

internal class MagicItemValidator : AbstractValidator<MagicItemModel>
{
  public MagicItemValidator()
  {
    When(x => x.Attunement is not null, () => RuleFor(x => x.Attunement!).SetValidator(new AttunementValidator()));
  }
}

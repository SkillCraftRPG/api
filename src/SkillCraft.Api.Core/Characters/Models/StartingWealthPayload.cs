using FluentValidation;

namespace SkillCraft.Api.Core.Characters.Models;

public record StartingWealthPayload
{
  public Guid ItemId { get; set; }
  public int Quantity { get; set; }
}

internal class StartingWealthValidator : AbstractValidator<StartingWealthPayload>
{
  public StartingWealthValidator()
  {
    RuleFor(x => x.Quantity).GreaterThan(0);
  }
}

using FluentValidation;

namespace SkillCraft.Api.Core.Lineages;

public interface ILineageSpeeds
{
  int? Walk { get; }
  int? Climb { get; }
  int? Swim { get; }
  int? Fly { get; }
  bool Hover { get; }
  int? Burrow { get; }
}

[method: JsonConstructor]
public record LineageSpeeds(int? Walk, int? Climb, int? Swim, int? Fly, bool Hover, int? Burrow) : ILineageSpeeds
{
  public LineageSpeeds(Lineage lineage) : this(lineage.Walk, lineage.Climb, lineage.Swim, lineage.Fly, lineage.Hover, lineage.Burrow)
  {
  }
}

internal class LineageSpeedsValidator : AbstractValidator<ILineageSpeeds>
{
  public LineageSpeedsValidator()
  {
    RuleFor(x => x.Walk).GreaterThan(0);
    RuleFor(x => x.Climb).GreaterThan(0);
    RuleFor(x => x.Swim).GreaterThan(0);
    RuleFor(x => x.Fly).GreaterThan(0);
    When(x => x.Hover, () => RuleFor(x => x.Fly).NotNull());
    RuleFor(x => x.Burrow).GreaterThan(0);
  }
}

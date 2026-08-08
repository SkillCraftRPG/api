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

public record LineageSpeeds : ILineageSpeeds
{
  public int? Walk { get; }
  public int? Climb { get; }
  public int? Swim { get; }
  public int? Fly { get; }
  public bool Hover { get; }
  public int? Burrow { get; }

  public LineageSpeeds()
  {
  }

  [JsonConstructor]
  public LineageSpeeds(int? walk, int? climb, int? swim, int? fly, bool hover, int? burrow)
  {
    Walk = walk;
    Climb = climb;
    Swim = swim;
    Fly = fly;
    Hover = hover;
    Burrow = burrow;
    new LineageSpeedsValidator().ValidateAndThrow(this);
  }

  public LineageSpeeds(ILineageSpeeds speeds) : this(speeds.Walk, speeds.Climb, speeds.Swim, speeds.Fly, speeds.Hover, speeds.Burrow)
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

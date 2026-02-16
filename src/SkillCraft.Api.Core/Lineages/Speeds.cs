using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core.Lineages.Validators;

namespace SkillCraft.Api.Core.Lineages;

public record Speeds : ISpeeds
{
  public int Walk { get; set; }
  public int Climb { get; set; }
  public int Swim { get; set; }
  public int Fly { get; set; }
  public bool Hover { get; set; }
  public int Burrow { get; set; }

  public Speeds() : this(0, 0, 0, 0, false, 0)
  {
  }

  public Speeds(ISpeeds speeds) : this(speeds.Walk, speeds.Climb, speeds.Swim, speeds.Fly, speeds.Hover, speeds.Burrow)
  {
  }

  public Speeds(int walk, int climb, int swim, int fly, bool hover, int burrow)
  {
    Walk = walk;
    Climb = climb;
    Swim = swim;
    Fly = fly;
    Hover = hover;
    Burrow = burrow;
    new SpeedsValidator().ValidateAndThrow(this);
  }
}

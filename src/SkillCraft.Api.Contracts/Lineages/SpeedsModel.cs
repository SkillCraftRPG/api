namespace SkillCraft.Api.Contracts.Lineages;

public record SpeedsModel : ISpeeds
{
  public int? Walk { get; set; }
  public int? Climb { get; set; }
  public int? Swim { get; set; }
  public int? Fly { get; set; }
  public bool Hover { get; set; }
  public int? Burrow { get; set; }

  public SpeedsModel(ISpeeds speeds) : this(speeds.Walk, speeds.Climb, speeds.Swim, speeds.Fly, speeds.Hover, speeds.Burrow)
  {
  }

  public SpeedsModel(int? walk = null, int? climb = null, int? swim = null, int? fly = null, bool hover = false, int? burrow = null)
  {
    Walk = walk;
    Climb = climb;
    Swim = swim;
    Fly = fly;
    Hover = hover;
    Burrow = burrow;
  }
}

namespace SkillCraft.Api.Contracts.Lineages;

public record SpeedsModel : ISpeeds
{
  public int Walk { get; set; }
  public int Climb { get; set; }
  public int Swim { get; set; }
  public int Fly { get; set; }
  public bool Hover { get; set; }
  public int Burrow { get; set; }

  public SpeedsModel() : this(0, 0, 0, 0, false, 0)
  {
  }

  public SpeedsModel(ISpeeds speeds) : this(speeds.Walk, speeds.Climb, speeds.Swim, speeds.Fly, speeds.Hover, speeds.Burrow)
  {
  }

  public SpeedsModel(int walk, int climb, int swim, int fly, bool hover, int burrow)
  {
    Walk = walk;
    Climb = climb;
    Swim = swim;
    Fly = fly;
    Hover = hover;
    Burrow = burrow;
  }
}

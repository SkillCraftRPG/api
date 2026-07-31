namespace SkillCraft.Api.Core.Lineages.Events;

public class LineageUpdated : UpdateEvent
{
  public Change<string>? Name { get; set; }
  public Change<string>? Summary { get; set; }
  public Change<string>? Content { get; set; }

  public Change<LineageLanguages>? Languages { get; set; }
  public Change<LineageNames>? Names { get; set; }
  public Change<LineageSpeeds>? Speeds { get; set; }
  public Change<LineageSize>? Size { get; set; }
  public Change<LineageWeight>? Weight { get; set; }
  public Change<LineageAge>? Age { get; set; }

  public LineageUpdated() : base()
  {
  }

  public LineageUpdated(Lineage lineage) : base(lineage)
  {
  }
}

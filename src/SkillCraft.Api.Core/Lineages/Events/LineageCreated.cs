namespace SkillCraft.Api.Core.Lineages.Events;

public class LineageCreated : CreateEvent
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Content { get; set; }

  public LineageLanguages Languages { get; set; } = new();
  public LineageNames Names { get; set; } = new();
  public LineageSpeeds Speeds { get; set; } = new();
  public LineageSize Size { get; set; } = new();
  public LineageWeight Weight { get; set; } = new();
  public LineageAge Age { get; set; } = new();

  public LineageCreated() : base()
  {
  }

  public LineageCreated(Lineage lineage) : base(lineage)
  {
    Name = lineage.Name;
    Summary = lineage.Summary;
    Content = lineage.Content;

    Languages = new LineageLanguages(lineage);
    Names = new LineageNames(lineage);
    Speeds = new LineageSpeeds(lineage);
    Size = new LineageSize(lineage);
    Weight = new LineageWeight(lineage);
    Age = new LineageAge(lineage);
  }
}

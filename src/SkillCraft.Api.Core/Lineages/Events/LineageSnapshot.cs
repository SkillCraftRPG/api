namespace SkillCraft.Api.Core.Lineages.Events;

public record LineageSnapshot
{
  public string Name { get; }
  public string? Summary { get; }
  public string? HtmlContent { get; }

  // TODO(fpion): Languages
  // TODO(fpion): Names
  // TODO(fpion): Speeds
  // TODO(fpion): Size
  // TODO(fpion): Weight
  // TODO(fpion): Age

  public bool HasChanges { get; private set; } // TODO(fpion): immplement

  public LineageSnapshot(Lineage lineage)
  {
    Name = lineage.Name;
    Summary = lineage.Summary;
    HtmlContent = lineage.HtmlContent;
  }
}

namespace SkillCraft.Api.Core.Lineages.Events;

public class LineageFeatureCreated : UpdateEvent
{
  public Guid FeatureId { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Content { get; set; }

  public LineageFeatureCreated() : base()
  {
  }

  public LineageFeatureCreated(LineageFeature feature) : base(feature.Lineage)
  {
    FeatureId = feature.Id;
    Name = feature.Name;
    Content = feature.Content;
  }
}

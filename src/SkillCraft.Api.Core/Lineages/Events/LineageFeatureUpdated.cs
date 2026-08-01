namespace SkillCraft.Api.Core.Lineages.Events;

public class LineageFeatureUpdated : UpdateEvent
{
  public Guid FeatureId { get; set; }
  public Change<string>? Name { get; set; }
  public Change<string>? Content { get; set; }

  public LineageFeatureUpdated() : base()
  {
  }

  public LineageFeatureUpdated(LineageFeature feature) : base(feature.Lineage)
  {
    FeatureId = feature.Id;
  }
}

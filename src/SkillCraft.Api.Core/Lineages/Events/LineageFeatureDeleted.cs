namespace SkillCraft.Api.Core.Lineages.Events;

public class LineageFeatureDeleted : UpdateEvent
{
  public Guid FeatureId { get; set; }

  public LineageFeatureDeleted() : base()
  {
  }

  public LineageFeatureDeleted(LineageFeature feature) : base(feature.Lineage)
  {
    FeatureId = feature.Id;
  }
}

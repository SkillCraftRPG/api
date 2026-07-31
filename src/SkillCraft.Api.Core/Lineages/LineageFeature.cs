using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Core.Lineages;

public class LineageFeature : IAuditable, IFeature
{
  public int LineageFeatureId { get; private set; }

  public Lineage? Lineage { get; private set; }
  public int LineageId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? HtmlContent { get; private set; }

  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  private LineageFeature()
  {
  }

  public override bool Equals(object? obj) => obj is LineageFeature feature && feature.LineageFeatureId == LineageFeatureId;
  public override int GetHashCode() => LineageFeatureId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (LineageFeatureId={LineageFeatureId})";
}

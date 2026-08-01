using Logitar;
using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Core.Lineages;

public class LineageFeature : IAuditable, IFeature
{
  public int LineageFeatureId { get; private set; }

  public Lineage? Lineage { get; private set; }
  public int LineageId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; set; } = string.Empty;
  public string? Content { get; set; }

  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public LineageFeature(Lineage lineage, Guid userId, Guid? id = null, DateTime? createdOn = null)
  {
    Lineage = lineage;
    LineageId = lineage.LineageId;
    Id = id ?? Guid.NewGuid();

    CreatedBy = UpdatedBy = userId;
    CreatedOn = UpdatedOn = (createdOn ?? DateTime.Now).AsUniversalTime();
  }

  private LineageFeature()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds() => [CreatedBy, UpdatedBy];

  public void Update(Guid userId, DateTime? updatedOn = null)
  {
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is LineageFeature feature && feature.LineageFeatureId == LineageFeatureId;
  public override int GetHashCode() => LineageFeatureId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (LineageFeatureId={LineageFeatureId})";
}

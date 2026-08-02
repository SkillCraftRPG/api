using Logitar;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class LineageFeatureEntity : IAuditable, IFeature
{
  public int LineageFeatureId { get; private set; }

  public LineageEntity? Lineage { get; private set; }
  public int LineageId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; set; } = string.Empty;
  public string? Content { get; set; }

  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public LineageFeatureEntity(LineageEntity lineage, Guid userId, Guid? id = null, DateTime? createdOn = null)
  {
    Lineage = lineage;
    LineageId = lineage.LineageId;
    Id = id ?? Guid.NewGuid();

    CreatedBy = UpdatedBy = userId;
    CreatedOn = UpdatedOn = (createdOn ?? DateTime.Now).AsUniversalTime();
  }

  private LineageFeatureEntity()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds() => [CreatedBy, UpdatedBy];

  public void Update(Guid userId, DateTime? updatedOn = null)
  {
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is LineageFeatureEntity feature && feature.LineageFeatureId == LineageFeatureId;
  public override int GetHashCode() => LineageFeatureId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (LineageFeatureId={LineageFeatureId})";
}

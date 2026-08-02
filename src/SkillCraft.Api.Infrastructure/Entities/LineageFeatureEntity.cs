using Logitar.EventSourcing;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class LineageFeatureEntity
{
  public int LineageFeatureId { get; private set; }

  public LineageEntity? Lineage { get; private set; }
  public int LineageId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; set; } = string.Empty;
  public string? Content { get; set; }

  public string? CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public string? UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  private LineageFeatureEntity()
  {
  }

  public virtual IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(capacity: 2);
    if (CreatedBy is not null)
    {
      actorIds.Add(new ActorId(CreatedBy));
    }
    if (UpdatedBy is not null)
    {
      actorIds.Add(new ActorId(UpdatedBy));
    }
    return actorIds.AsReadOnly();
  }

  public override bool Equals(object? obj) => obj is LineageFeatureEntity feature && feature.LineageFeatureId == LineageFeatureId;
  public override int GetHashCode() => LineageFeatureId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (LineageFeatureId={LineageFeatureId})";
}

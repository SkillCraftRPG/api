using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Lineages;

public readonly struct LineageId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid ResourceId { get; }

  public LineageId(StreamId streamId)
  {
    StreamId = streamId;

    ResourceIdentifier resource = ResourceIdentifier.Parse(streamId.Value);
    WorldId = resource.WorldId ?? throw new ArgumentException("A world is required.", nameof(streamId));
    ResourceId = resource.Id;
  }

  public LineageId(string value) : this(new StreamId(value))
  {
  }

  public LineageId(WorldId worldId, Guid resourceId)
  {
    ResourceIdentifier resource = new(Lineage.ResourceKind, resourceId, worldId);
    StreamId = new StreamId(resource.ToString());

    WorldId = worldId;
    ResourceId = resourceId;
  }

  public static LineageId NewId(WorldId worldId) => new(worldId, Guid.NewGuid());

  public static bool operator ==(LineageId left, LineageId right) => left.Equals(right);
  public static bool operator !=(LineageId left, LineageId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is LineageId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

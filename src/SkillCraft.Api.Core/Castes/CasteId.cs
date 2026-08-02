using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Castes;

public readonly struct CasteId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid ResourceId { get; }

  public CasteId(StreamId streamId)
  {
    StreamId = streamId;

    ResourceIdentifier resource = ResourceIdentifier.Parse(streamId.Value);
    WorldId = resource.WorldId ?? throw new ArgumentException("A world is required.", nameof(streamId));
    ResourceId = resource.Id;
  }

  public CasteId(string value) : this(new StreamId(value))
  {
  }

  public CasteId(WorldId worldId, Guid resourceId)
  {
    ResourceIdentifier resource = new(Caste.ResourceKind, resourceId, worldId);
    StreamId = new StreamId(resource.ToString());

    WorldId = worldId;
    ResourceId = resourceId;
  }

  public static CasteId NewId(WorldId worldId) => new(worldId, Guid.NewGuid());

  public static bool operator ==(CasteId left, CasteId right) => left.Equals(right);
  public static bool operator !=(CasteId left, CasteId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is CasteId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

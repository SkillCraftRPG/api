using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Castes;

public readonly struct CasteId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid EntityId { get; }

  public CasteId(StreamId streamId)
  {
    StreamId = streamId;

    Entity entity = Entity.Parse(streamId.Value, Caste.EntityKind);
    if (!entity.WorldId.HasValue)
    {
      throw new ArgumentException("A world identifier is required.", nameof(streamId));
    }
    WorldId = entity.WorldId.Value;
    EntityId = entity.Id;
  }

  public CasteId(Guid entityId, WorldId worldId)
  {
    Entity entity = new(Caste.EntityKind, entityId, worldId);
    StreamId = new StreamId(entity.ToString());

    WorldId = worldId;
    EntityId = entityId;
  }

  public CasteId(string value) : this(new StreamId(value))
  {
  }

  public static CasteId NewId(WorldId worldId) => new(Guid.NewGuid(), worldId);

  public static bool operator ==(CasteId left, CasteId right) => left.Equals(right);
  public static bool operator !=(CasteId left, CasteId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is CasteId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

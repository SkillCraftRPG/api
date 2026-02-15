using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Parties;

public readonly struct PartyId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid EntityId { get; }

  public PartyId(StreamId streamId)
  {
    StreamId = streamId;

    Entity entity = Entity.Parse(streamId.Value, Party.EntityKind);
    if (!entity.WorldId.HasValue)
    {
      throw new ArgumentException("A world identifier is required.", nameof(streamId));
    }
    WorldId = entity.WorldId.Value;
    EntityId = entity.Id;
  }

  public PartyId(Guid entityId, WorldId worldId)
  {
    Entity entity = new(Party.EntityKind, entityId, worldId);
    StreamId = new StreamId(entity.ToString());

    WorldId = worldId;
    EntityId = entityId;
  }

  public PartyId(string value) : this(new StreamId(value))
  {
  }

  public static PartyId NewId(WorldId worldId) => new(Guid.NewGuid(), worldId);

  public static bool operator ==(PartyId left, PartyId right) => left.Equals(right);
  public static bool operator !=(PartyId left, PartyId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is PartyId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Characters;

public readonly struct CharacterId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid EntityId { get; }

  public CharacterId(StreamId streamId)
  {
    StreamId = streamId;

    Entity entity = Entity.Parse(streamId.Value, Character.EntityKind);
    if (!entity.WorldId.HasValue)
    {
      throw new ArgumentException("A world identifier is required.", nameof(streamId));
    }
    WorldId = entity.WorldId.Value;
    EntityId = entity.Id;
  }

  public CharacterId(Guid entityId, WorldId worldId)
  {
    Entity entity = new(Character.EntityKind, entityId, worldId);
    StreamId = new StreamId(entity.ToString());

    WorldId = worldId;
    EntityId = entityId;
  }

  public CharacterId(string value) : this(new StreamId(value))
  {
  }

  public static CharacterId NewId(WorldId worldId) => new(Guid.NewGuid(), worldId);

  public static bool operator ==(CharacterId left, CharacterId right) => left.Equals(right);
  public static bool operator !=(CharacterId left, CharacterId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is CharacterId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

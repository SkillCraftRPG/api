using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Scripts;

public readonly struct ScriptId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid EntityId { get; }

  public ScriptId(StreamId streamId)
  {
    StreamId = streamId;

    Entity entity = Entity.Parse(streamId.Value, Script.EntityKind);
    if (!entity.WorldId.HasValue)
    {
      throw new ArgumentException("A world identifier is required.", nameof(streamId));
    }
    WorldId = entity.WorldId.Value;
    EntityId = entity.Id;
  }

  public ScriptId(Guid entityId, WorldId worldId)
  {
    Entity entity = new(Script.EntityKind, entityId, worldId);
    StreamId = new StreamId(entity.ToString());

    WorldId = worldId;
    EntityId = entityId;
  }

  public ScriptId(string value) : this(new StreamId(value))
  {
  }

  public static ScriptId NewId(WorldId worldId) => new(Guid.NewGuid(), worldId);

  public static bool operator ==(ScriptId left, ScriptId right) => left.Equals(right);
  public static bool operator !=(ScriptId left, ScriptId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ScriptId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

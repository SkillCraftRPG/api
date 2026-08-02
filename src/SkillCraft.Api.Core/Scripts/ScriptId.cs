using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Scripts;

public readonly struct ScriptId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid ResourceId { get; }

  public ScriptId(StreamId streamId)
  {
    StreamId = streamId;

    Resource resource = Resource.Parse(streamId.Value);
    WorldId = resource.WorldId ?? throw new ArgumentException("A world is required.", nameof(streamId));
    ResourceId = resource.Id;
  }

  public ScriptId(string value) : this(new StreamId(value))
  {
  }

  public ScriptId(WorldId worldId, Guid resourceId)
  {
    Resource resource = new(Script.ResourceKind, resourceId, worldId);
    StreamId = new StreamId(resource.ToString());

    WorldId = worldId;
    ResourceId = resourceId;
  }

  public static ScriptId NewId(WorldId worldId) => new(worldId, Guid.NewGuid());

  public static bool operator ==(ScriptId left, ScriptId right) => left.Equals(right);
  public static bool operator !=(ScriptId left, ScriptId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ScriptId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

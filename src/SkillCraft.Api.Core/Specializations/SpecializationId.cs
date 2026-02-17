using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Specializations;

public readonly struct SpecializationId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid EntityId { get; }

  public SpecializationId(StreamId streamId)
  {
    StreamId = streamId;

    Entity entity = Entity.Parse(streamId.Value, Specialization.EntityKind);
    if (!entity.WorldId.HasValue)
    {
      throw new ArgumentException("A world identifier is required.", nameof(streamId));
    }
    WorldId = entity.WorldId.Value;
    EntityId = entity.Id;
  }

  public SpecializationId(Guid entityId, WorldId worldId)
  {
    Entity entity = new(Specialization.EntityKind, entityId, worldId);
    StreamId = new StreamId(entity.ToString());

    WorldId = worldId;
    EntityId = entityId;
  }

  public SpecializationId(string value) : this(new StreamId(value))
  {
  }

  public static SpecializationId NewId(WorldId worldId) => new(Guid.NewGuid(), worldId);

  public static bool operator ==(SpecializationId left, SpecializationId right) => left.Equals(right);
  public static bool operator !=(SpecializationId left, SpecializationId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is SpecializationId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

using System.Diagnostics.CodeAnalysis;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Educations;

public readonly struct EducationId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid EntityId { get; }

  public EducationId(StreamId streamId)
  {
    StreamId = streamId;

    Entity entity = Entity.Parse(streamId.Value, Education.EntityKind);
    if (!entity.WorldId.HasValue)
    {
      throw new ArgumentException("A world identifier is required.", nameof(streamId));
    }
    WorldId = entity.WorldId.Value;
    EntityId = entity.Id;
  }

  public EducationId(Guid entityId, WorldId worldId)
  {
    Entity entity = new(Education.EntityKind, entityId, worldId);
    StreamId = new StreamId(entity.ToString());

    WorldId = worldId;
    EntityId = entityId;
  }

  public EducationId(string value) : this(new StreamId(value))
  {
  }

  public static EducationId NewId(WorldId worldId) => new(Guid.NewGuid(), worldId);

  public static bool operator ==(EducationId left, EducationId right) => left.Equals(right);
  public static bool operator !=(EducationId left, EducationId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is EducationId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

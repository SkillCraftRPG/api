using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Educations;

public readonly struct EducationId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid ResourceId { get; }

  public EducationId(StreamId streamId)
  {
    StreamId = streamId;

    Resource resource = Resource.Parse(streamId.Value);
    WorldId = resource.WorldId ?? throw new ArgumentException("A world is required.", nameof(streamId));
    ResourceId = resource.Id;
  }

  public EducationId(string value) : this(new StreamId(value))
  {
  }

  public EducationId(WorldId worldId, Guid resourceId)
  {
    Resource resource = new(Education.ResourceKind, resourceId, worldId);
    StreamId = new StreamId(resource.ToString());

    WorldId = worldId;
    ResourceId = resourceId;
  }

  public static EducationId NewId(WorldId worldId) => new(worldId, Guid.NewGuid());

  public static bool operator ==(EducationId left, EducationId right) => left.Equals(right);
  public static bool operator !=(EducationId left, EducationId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is EducationId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

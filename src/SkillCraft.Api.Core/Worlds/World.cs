using Logitar;

namespace SkillCraft.Api.Core.Worlds;

public class World : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "World";

  public int WorldId { get; private set; }
  public Guid Id { get; private set; }

  public Guid OwnerId { get; private set; }

  public string Key { get; private set; } = string.Empty;
  public string? Name { get; private set; }
  public string? Description { get; private set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id);

  public World(Guid ownerId, string key, Guid? id = null, string? name = null, string? description = null, DateTime? createdOn = null)
  {
    createdOn = (createdOn ?? DateTime.Now).AsUniversalTime();

    Id = id ?? Guid.NewGuid();

    OwnerId = ownerId;

    CreatedBy = ownerId;
    CreatedOn = createdOn.Value;

    Update(key, name, description, ownerId, createdOn);
  }

  private World()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds() => [OwnerId, CreatedBy, UpdatedBy];

  public void Update(string key, string? name, string? description, Guid userId, DateTime? updatedOn = null)
  {
    Key = SlugHelper.Format(key);
    Name = name?.CleanTrim();
    Description = description?.CleanTrim();

    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is World world && world.WorldId == WorldId;
  public override int GetHashCode() => WorldId.GetHashCode();
  public override string ToString() => $"{Name ?? Key} | {GetType()} (WorldId={WorldId})";
}

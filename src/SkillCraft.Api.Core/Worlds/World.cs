using Logitar;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds.Events;

namespace SkillCraft.Api.Core.Worlds;

public class World : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "World";

  public int WorldId { get; private set; }
  public Guid Id { get; private set; }

  public Guid OwnerId { get; private set; }

  public string Key { get; private set; } = string.Empty;
  public string? Name { get; private set; }
  public string? HtmlContent { get; private set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id);

  public List<Caste> Castes { get; private set; } = [];
  public List<Customization> Customizations { get; private set; } = [];
  public List<Language> Languages { get; private set; } = [];
  public List<Script> Scripts { get; private set; } = [];

  public World(Guid ownerId, string key, Guid? id = null, string? name = null, string? htmlContent = null, DateTime? createdOn = null)
  {
    createdOn = (createdOn ?? DateTime.Now).AsUniversalTime();

    Id = id ?? Guid.NewGuid();

    OwnerId = ownerId;

    CreatedBy = ownerId;
    CreatedOn = createdOn.Value;

    Update(key, name, htmlContent, ownerId, createdOn);
  }

  private World()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds() => [OwnerId, CreatedBy, UpdatedBy];

  public WorldUpdated Update(string key, string? name, string? htmlContent, Guid userId, DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();

    WorldUpdated record = new(this);

    key = SlugHelper.Format(key);
    if (!Equals(Key, key))
    {
      record.Key = new Change<string>(Key, key);
      Key = key;
    }

    name = name?.CleanTrim();
    if (!Equals(Name, name))
    {
      record.Name = new Change<string>(Name, name);
      Name = name;
    }

    htmlContent = htmlContent?.CleanTrim();
    if (!Equals(HtmlContent, htmlContent))
    {
      record.HtmlContent = new Change<string>(HtmlContent, htmlContent);
      HtmlContent = htmlContent;
    }

    return record;
  }

  public override bool Equals(object? obj) => obj is World world && world.WorldId == WorldId;
  public override int GetHashCode() => WorldId.GetHashCode();
  public override string ToString() => $"{Name ?? Key} | {GetType()} (WorldId={WorldId})";
}

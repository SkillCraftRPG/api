using Logitar;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Core.Worlds;

public class World : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "World";

  public int WorldId { get; private set; }
  public Guid Id { get; private set; }

  public Guid OwnerId { get; private set; }

  public string Key { get; set; } = string.Empty;
  public string? Name { get; set; }
  public string? HtmlContent { get; set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id);

  public List<Caste> Castes { get; private set; } = [];
  public List<Customization> Customizations { get; private set; } = [];
  public List<Education> Educations { get; private set; } = [];
  public List<Item> Items { get; private set; } = [];
  public List<Language> Languages { get; private set; } = [];
  public List<Script> Scripts { get; private set; } = [];
  public List<Talent> Talents { get; private set; } = [];

  public World(Guid ownerId, Guid? id = null, DateTime? createdOn = null)
  {
    Id = id ?? Guid.NewGuid();

    OwnerId = ownerId;

    Version = 1;
    CreatedBy = UpdatedBy = ownerId;
    CreatedOn = UpdatedOn = (createdOn ?? DateTime.Now).AsUniversalTime();
  }

  private World()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds() => [OwnerId, CreatedBy, UpdatedBy];

  public void Update(Guid userId, DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is World world && world.WorldId == WorldId;
  public override int GetHashCode() => WorldId.GetHashCode();
  public override string ToString() => $"{Name ?? Key} | {GetType()} (WorldId={WorldId})";
}

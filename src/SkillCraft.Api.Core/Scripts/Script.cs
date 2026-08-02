using Logitar;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Scripts;

public class Script : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "Script";

  public int ScriptId { get; private set; }

  // TODO(fpion): public WorldEntity? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Content { get; set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id, WorldId);

  public List<Language> Languages { get; private set; } = [];

  public Script(World world, Guid? id = null, Guid? userId = null, DateTime? createdOn = null)
  {
    // TODO(fpion): World = world;
    WorldId = world.ResourceId;
    Id = id ?? Guid.NewGuid();

    Version = 1;
    CreatedBy = UpdatedBy = userId ?? world.OwnerId.ResourceId;
    CreatedOn = UpdatedOn = (createdOn ?? DateTime.Now).AsUniversalTime();
  }

  private Script()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds() => [CreatedBy, UpdatedBy];

  public void Update(Guid userId, DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is Script script && script.ScriptId == ScriptId;
  public override int GetHashCode() => ScriptId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (ScriptId={ScriptId})";
}

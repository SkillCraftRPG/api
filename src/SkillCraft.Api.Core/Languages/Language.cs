using Logitar;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Languages;

public class Language : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "Language";

  public int LanguageId { get; private set; }

  // TODO(fpion): public WorldEntity? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Content { get; set; }

  public Script? Script { get; private set; }
  public int? ScriptId { get; private set; }
  public string? TypicalSpeakers { get; set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id, WorldId);

  public List<Lineage> Lineages { get; private set; } = [];

  public Language(World world, Guid? id = null, Guid? userId = null, DateTime? createdOn = null)
  {
    // TODO(fpion): World = world;
    WorldId = world.ResourceId;
    Id = id ?? Guid.NewGuid();

    Version = 1;
    CreatedBy = UpdatedBy = userId ?? world.OwnerId.ResourceId;
    CreatedOn = UpdatedOn = (createdOn ?? DateTime.Now).AsUniversalTime();
  }

  private Language()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds()
  {
    List<Guid> userIds = [CreatedBy, UpdatedBy];
    if (Script is not null)
    {
      userIds.AddRange(Script.GetUserIds());
    }
    return userIds.AsReadOnly();
  }

  public void SetScript(Script? script)
  {
    Script = script;
    ScriptId = script?.ScriptId;
  }

  public void Update(Guid userId, DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is Language language && language.LanguageId == LanguageId;
  public override int GetHashCode() => LanguageId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (LanguageId={LanguageId})";
}

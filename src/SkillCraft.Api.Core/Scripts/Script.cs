using Logitar;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Scripts.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Scripts;

public class Script : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "Script";

  public int ScriptId { get; private set; }

  public World? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? HtmlContent { get; private set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id, WorldId);

  public List<Language> Languages { get; private set; } = [];

  public Script(
    World world,
    string name,
    Guid? id = null,
    string? summary = null,
    string? htmlContent = null,
    Guid? userId = null,
    DateTime? createdOn = null)
  {
    createdOn = (createdOn ?? DateTime.Now).AsUniversalTime();
    userId ??= world.OwnerId;

    World = world;
    WorldId = world.Id;
    Id = id ?? Guid.NewGuid();

    CreatedBy = userId.Value;
    CreatedOn = createdOn.Value;

    Update(name, summary, htmlContent, userId.Value, createdOn);
  }

  private Script()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds() => [CreatedBy, UpdatedBy];

  public ScriptUpdated Update(string name, string? summary, string? htmlContent, Guid userId, DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();

    ScriptUpdated record = new(this);

    name = name.CleanTrim() ?? string.Empty;
    if (!Equals(Name, name))
    {
      record.Name = new Change<string>(Name, name);
      Name = name;
    }

    summary = summary?.CleanTrim();
    if (!Equals(Summary, summary))
    {
      record.Summary = new Change<string>(Summary, summary);
      Summary = summary;
    }

    htmlContent = htmlContent?.CleanTrim();
    if (!Equals(HtmlContent, htmlContent))
    {
      record.HtmlContent = new Change<string>(HtmlContent, htmlContent);
      HtmlContent = htmlContent;
    }

    return record;
  }

  public override bool Equals(object? obj) => obj is Script script && script.ScriptId == ScriptId;
  public override int GetHashCode() => ScriptId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (ScriptId={ScriptId})";
}

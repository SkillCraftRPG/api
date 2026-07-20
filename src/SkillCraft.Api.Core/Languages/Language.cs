using Logitar;
using SkillCraft.Api.Core.Languages.Events;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Languages;

public class Language : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "Language";

  public int LanguageId { get; private set; }

  public World? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? HtmlContent { get; private set; }

  public Script? Script { get; private set; }
  public int? ScriptId { get; private set; }
  public string? TypicalSpeakers { get; private set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id, WorldId);

  public Language(
    World world,
    string name,
    Guid? id = null,
    string? summary = null,
    string? htmlContent = null,
    Script? script = null,
    string? typicalSpeakers = null,
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

    Update(name, summary, htmlContent, script, typicalSpeakers, userId.Value, createdOn);
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

  public LanguageUpdated Update(
    string name,
    string? summary,
    string? htmlContent,
    Script? script,
    string? typicalSpeakers,
    Guid userId,
    DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();

    LanguageUpdated record = new(this);

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

    if (!Equals(Script?.Id, script?.Id))
    {
      record.ScriptId = new Change<Guid?>(Script?.Id, script?.Id);
      Script = script;
      ScriptId = script?.ScriptId;
    }

    typicalSpeakers = typicalSpeakers?.CleanTrim();
    if (!Equals(TypicalSpeakers, typicalSpeakers))
    {
      record.TypicalSpeakers = new Change<string>(TypicalSpeakers, typicalSpeakers);
      TypicalSpeakers = typicalSpeakers;
    }

    return record;
  }

  public override bool Equals(object? obj) => obj is Language language && language.LanguageId == LanguageId;
  public override int GetHashCode() => LanguageId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (LanguageId={LanguageId})";
}

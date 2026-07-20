using Logitar;
using SkillCraft.Api.Core.Castes.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Castes;

public class Caste : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "Caste";

  public int CasteId { get; private set; }

  public World? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? HtmlContent { get; private set; }

  public Skill? Skill { get; private set; }
  public string? WealthRoll { get; private set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id, WorldId);

  public Caste(
    World world,
    string name,
    Guid? id = null,
    string? summary = null,
    string? htmlContent = null,
    Skill? skill = null,
    string? wealthRoll = null,
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

    Update(name, summary, htmlContent, skill, wealthRoll, userId.Value, createdOn);
  }

  private Caste()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds() => [CreatedBy, UpdatedBy];

  public CasteUpdated Update(
    string name,
    string? summary,
    string? htmlContent,
    Skill? skill,
    string? wealthRoll,
    Guid userId,
    DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();

    CasteUpdated record = new(this);

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

    if (!Equals(Skill, skill))
    {
      record.Skill = new Change<Skill?>(Skill, skill);
      Skill = skill;
    }

    wealthRoll = wealthRoll?.CleanTrim();
    if (!Equals(WealthRoll, wealthRoll))
    {
      record.WealthRoll = new Change<string>(WealthRoll, wealthRoll);
      WealthRoll = wealthRoll;
    }

    return record;
  }

  public override bool Equals(object? obj) => obj is Caste caste && caste.CasteId == CasteId;
  public override int GetHashCode() => CasteId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (CasteId={CasteId})";
}

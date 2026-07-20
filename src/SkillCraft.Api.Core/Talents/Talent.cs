using Logitar;
using SkillCraft.Api.Core.Talents.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents;

public class Talent : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "Talent";

  public int TalentId { get; private set; }

  public World? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public int Tier { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? HtmlContent { get; private set; }

  public bool AllowMultiplePurchases { get; private set; }
  public Skill? Skill { get; private set; }

  public Talent? RequiredTalent { get; private set; }
  public int? RequiredTalentId { get; private set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id, WorldId);

  public List<Talent> RequiringTalents { get; private set; } = [];

  public Talent(
    World world,
    int tier,
    string name,
    Guid? id = null,
    string? summary = null,
    string? htmlContent = null,
    bool allowMultiplePurchases = false,
    Skill? skill = null,
    Talent? requiredTalent = null,
    Guid? userId = null,
    DateTime? createdOn = null)
  {
    createdOn = (createdOn ?? DateTime.Now).AsUniversalTime();
    userId ??= world.OwnerId;

    World = world;
    WorldId = world.Id;
    Id = id ?? Guid.NewGuid();

    Tier = tier;

    CreatedBy = userId.Value;
    CreatedOn = createdOn.Value;

    Update(name, summary, htmlContent, allowMultiplePurchases, skill, requiredTalent, userId.Value, createdOn);
  }

  private Talent()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds()
  {
    List<Guid> userIds = [CreatedBy, UpdatedBy];
    if (RequiredTalent is not null)
    {
      userIds.AddRange(RequiredTalent.GetUserIds());
    }
    return userIds.AsReadOnly();
  }

  public TalentUpdated Update(
    string name,
    string? summary,
    string? htmlContent,
    bool allowMultiplePurchases,
    Skill? skill,
    Talent? requiredTalent,
    Guid userId,
    DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();

    TalentUpdated record = new(this);

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

    if (AllowMultiplePurchases != allowMultiplePurchases)
    {
      record.AllowMultiplePurchases = new Change<bool>(AllowMultiplePurchases, allowMultiplePurchases);
      AllowMultiplePurchases = allowMultiplePurchases;
    }

    if (!Equals(Skill, skill))
    {
      record.Skill = new Change<Skill?>(Skill, skill);
      Skill = skill;
    }

    if (!Equals(RequiredTalent?.Id, requiredTalent?.Id))
    {
      record.RequiredTalentId = new Change<Guid?>(RequiredTalent?.Id, requiredTalent?.Id);
      RequiredTalent = requiredTalent;
      RequiredTalentId = requiredTalent?.TalentId;
    }

    return record;
  }

  public override bool Equals(object? obj) => obj is Talent talent && talent.TalentId == TalentId;
  public override int GetHashCode() => TalentId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (TalentId={TalentId})";
}

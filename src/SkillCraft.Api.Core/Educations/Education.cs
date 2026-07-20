using Logitar;
using SkillCraft.Api.Core.Educations.Events;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Educations;

public class Education : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "Education";

  public int EducationId { get; private set; }

  public World? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? HtmlContent { get; private set; }

  public Skill? Skill { get; private set; }
  public int? WealthMultiplier { get; private set; }

  public string? FeatureName { get; private set; }
  public string? FeatureHtmlContent { get; private set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id, WorldId);

  public Education(
    World world,
    string name,
    Guid? id = null,
    string? summary = null,
    string? htmlContent = null,
    Skill? skill = null,
    int? wealthMultiplier = null,
    Feature? feature = null,
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

    Update(name, summary, htmlContent, skill, wealthMultiplier, feature, userId.Value, createdOn);
  }

  private Education()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds() => [CreatedBy, UpdatedBy];

  public EducationUpdated Update(
    string name,
    string? summary,
    string? htmlContent,
    Skill? skill,
    int? wealthMultiplier,
    Feature? feature,
    Guid userId,
    DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();

    EducationUpdated record = new(this);

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

    if (!Equals(WealthMultiplier, wealthMultiplier))
    {
      record.WealthMultiplier = new Change<int?>(WealthMultiplier, wealthMultiplier);
      WealthMultiplier = wealthMultiplier;
    }

    Feature? currentFeature = FeatureName is null ? null : new(FeatureName, FeatureHtmlContent);
    if (!Equals(currentFeature, feature))
    {
      record.Feature = new Change<Feature>(currentFeature, feature);
      FeatureName = feature?.Name;
      FeatureHtmlContent = feature?.HtmlContent;
    }

    return record;
  }

  public override bool Equals(object? obj) => obj is Education education && education.EducationId == EducationId;
  public override int GetHashCode() => EducationId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (EducationId={EducationId})";
}

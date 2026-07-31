using Logitar;
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

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public Skill? Skill { get; set; }
  public int? WealthMultiplier { get; set; }

  public string? FeatureName { get; private set; }
  public string? FeatureHtmlContent { get; private set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id, WorldId);

  public Education(World world, Guid? id = null, Guid? userId = null, DateTime? createdOn = null)
  {
    World = world;
    WorldId = world.Id;
    Id = id ?? Guid.NewGuid();

    Version = 1;
    CreatedBy = UpdatedBy = userId ?? world.OwnerId;
    CreatedOn = UpdatedOn = (createdOn ?? DateTime.Now).AsUniversalTime();
  }

  private Education()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds() => [CreatedBy, UpdatedBy];

  public void SetFeature(Feature? feature)
  {
    FeatureName = feature?.Name;
    FeatureHtmlContent = feature?.HtmlContent;
  }

  public void Update(Guid userId, DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is Education education && education.EducationId == EducationId;
  public override int GetHashCode() => EducationId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (EducationId={EducationId})";
}

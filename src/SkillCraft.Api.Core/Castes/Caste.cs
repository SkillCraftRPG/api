using Logitar;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Castes;

public class Caste : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "Caste";

  public int CasteId { get; private set; }

  // TODO(fpion): public WorldEntity? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Content { get; set; }

  public Skill? Skill { get; set; }
  public string? WealthRoll { get; set; }

  public string? FeatureName { get; private set; }
  public string? FeatureContent { get; private set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id, WorldId);

  public Caste(World world, Guid? id = null, Guid? userId = null, DateTime? createdOn = null)
  {
    // TODO(fpion): World = world;
    WorldId = world.ResourceId;
    Id = id ?? Guid.NewGuid();

    Version = 1;
    CreatedBy = UpdatedBy = userId ?? world.OwnerId.ResourceId;
    CreatedOn = UpdatedOn = (createdOn ?? DateTime.Now).AsUniversalTime();
  }

  private Caste()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds() => [CreatedBy, UpdatedBy];

  public void SetFeature(Feature? feature)
  {
    FeatureName = feature?.Name;
    FeatureContent = feature?.Content;
  }

  public void Update(Guid userId, DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is Caste caste && caste.CasteId == CasteId;
  public override int GetHashCode() => CasteId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (CasteId={CasteId})";
}

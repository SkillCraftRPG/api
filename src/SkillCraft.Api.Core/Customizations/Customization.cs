using Logitar;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Customizations;

public class Customization : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "Customization";

  public int CustomizationId { get; private set; }

  public World? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public CustomizationKind Kind { get; private set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Content { get; set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id, WorldId);

  public Customization(World world, CustomizationKind kind, Guid? id = null, Guid? userId = null, DateTime? createdOn = null)
  {
    World = world;
    WorldId = world.Id;
    Id = id ?? Guid.NewGuid();

    Kind = kind;

    Version = 1;
    CreatedBy = UpdatedBy = userId ?? world.OwnerId;
    CreatedOn = UpdatedOn = (createdOn ?? DateTime.Now).AsUniversalTime();
  }

  private Customization()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds() => [CreatedBy, UpdatedBy];

  public void Update(Guid userId, DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is Customization customization && customization.CustomizationId == CustomizationId;
  public override int GetHashCode() => CustomizationId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (CustomizationId={CustomizationId})";
}

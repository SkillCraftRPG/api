using Logitar;
using SkillCraft.Api.Core.Customizations.Events;
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

  public string Name { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id);

  public Customization(
    World world,
    CustomizationKind kind,
    string name,
    Guid? id = null,
    string? description = null,
    Guid? userId = null,
    DateTime? createdOn = null)
  {
    createdOn = (createdOn ?? DateTime.Now).AsUniversalTime();
    userId ??= world.OwnerId;

    World = world;
    WorldId = world.Id;
    Id = id ?? Guid.NewGuid();

    Kind = kind;

    CreatedBy = userId.Value;
    CreatedOn = createdOn.Value;

    Update(name, description, userId.Value, createdOn);
  }

  private Customization()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds() => [CreatedBy, UpdatedBy];

  public CustomizationUpdated Update(string name, string? description, Guid userId, DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();

    CustomizationUpdated record = new(this);

    name = name.CleanTrim() ?? string.Empty;
    if (!Equals(Name, name))
    {
      record.Name = new Change<string>(Name, name);
      Name = name;
    }

    description = description?.CleanTrim();
    if (!Equals(Description, description))
    {
      record.Description = new Change<string>(Description, description);
      Description = description;
    }

    return record;
  }

  public override bool Equals(object? obj) => obj is Customization customization && customization.CustomizationId == CustomizationId;
  public override int GetHashCode() => CustomizationId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (CustomizationId={CustomizationId})";
}

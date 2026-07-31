using Logitar;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Lineages;

public class Lineage : IAuditable, IResource, IVersioned
{
  public const string ResourceKind = "Lineage";

  public int LineageId { get; private set; }

  public World? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public Lineage? Parent { get; private set; }
  public List<Lineage> Children { get; private set; } = [];
  public int? ParentId { get; private set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public int ExtraLanguages { get; set; }
  public string? LanguagesHtmlContent { get; set; }

  public string? FamilyNames { get; set; }
  public string? FemaleNames { get; set; }
  public string? MaleNames { get; set; }
  public string? UnisexNames { get; set; }
  public string? CustomNames { get; set; }
  public string? NamesHtmlContent { get; set; }

  public int Walk { get; set; }
  public int Climb { get; set; }
  public int Swim { get; set; }
  public int Fly { get; set; }
  public bool Hover { get; set; }
  public int Burrow { get; set; }

  public SizeCategory SizeCategory { get; set; }
  public string? HeightRoll { get; set; }

  public string? Malnutrition { get; set; }
  public string? Skinny { get; set; }
  public string? NormalWeight { get; set; }
  public string? Overweight { get; set; }
  public string? Obese { get; set; }

  public int Teenager { get; set; }
  public int Adult { get; set; }
  public int Mature { get; set; }
  public int Venerable { get; set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, Id, WorldId);

  public List<LineageFeature> Features { get; private set; } = [];
  public List<Language> Languages { get; private set; } = [];

  public Lineage(World world, Guid? id = null, Lineage? parent = null, Guid? userId = null, DateTime? createdOn = null)
  {
    World = world;
    WorldId = world.Id;
    Id = id ?? Guid.NewGuid();

    Parent = parent;
    ParentId = parent?.LineageId;

    Version = 1;
    CreatedBy = UpdatedBy = userId ?? world.OwnerId;
    CreatedOn = UpdatedOn = (createdOn ?? DateTime.Now).AsUniversalTime();
  }

  private Lineage()
  {
  }

  public void Update(Guid userId, DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is Lineage lineage && lineage.LineageId == LineageId;
  public override int GetHashCode() => LineageId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (LineageId={LineageId})";
}

using Logitar;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages.Models;
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
  public string? Content { get; set; }

  public int ExtraLanguages { get; private set; }
  public string? LanguagesContent { get; private set; }

  public string? FamilyNames { get; private set; }
  public string? FemaleNames { get; private set; }
  public string? MaleNames { get; private set; }
  public string? UnisexNames { get; private set; }
  public string? CustomNames { get; private set; }
  public string? NamesContent { get; private set; }

  public int? Walk { get; private set; }
  public int? Climb { get; private set; }
  public int? Swim { get; private set; }
  public int? Fly { get; private set; }
  public bool Hover { get; private set; }
  public int? Burrow { get; private set; }

  public SizeCategory SizeCategory { get; private set; }
  public string? HeightRoll { get; private set; }

  public string? Malnutrition { get; private set; }
  public string? Skinny { get; private set; }
  public string? NormalWeight { get; private set; }
  public string? Overweight { get; private set; }
  public string? Obese { get; private set; }

  public int? Teenager { get; private set; }
  public int? Adult { get; private set; }
  public int? Mature { get; private set; }
  public int? Venerable { get; private set; }

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
    if (parent?.Parent is not null)
    {
      throw new InvalidParentLineageException(parent, nameof(Lineage.ParentId));
    }

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

  public IReadOnlyCollection<Guid> GetUserIds()
  {
    List<Guid> userIds = [CreatedBy, UpdatedBy];
    foreach (LineageFeature feature in Features)
    {
      userIds.AddRange(feature.GetUserIds());
    }
    foreach (Language language in Languages)
    {
      userIds.AddRange(language.GetUserIds());
    }
    return userIds.AsReadOnly();
  }

  public void SetAge(ILineageAge age)
  {
    Teenager = age.Teenager;
    Adult = age.Adult;
    Mature = age.Mature;
    Venerable = age.Venerable;
  }

  public void SetLanguages(IEnumerable<Language> granted, int extra, string? content)
  {
    Languages.Clear();
    Languages.AddRange(granted);
    ExtraLanguages = extra;
    LanguagesContent = content?.CleanTrim();
  }

  public void SetNames(
    IEnumerable<string> family,
    IEnumerable<string> female,
    IEnumerable<string> male,
    IEnumerable<string> unisex,
    IEnumerable<NameCategory> custom,
    string? content)
  {
    FamilyNames = EncodeNames(family);
    FemaleNames = EncodeNames(female);
    MaleNames = EncodeNames(male);
    UnisexNames = EncodeNames(unisex);

    Dictionary<string, IEnumerable<string>> customNames = new(capacity: custom.Count());
    foreach (NameCategory nameCategory in custom)
    {
      string category = nameCategory.Category.Trim();
      IEnumerable<string> cleaned = CleanNames(nameCategory.Values);
      if (!string.IsNullOrEmpty(category) && cleaned.Any())
      {
        customNames[category] = cleaned;
      }
    }
    CustomNames = customNames.Count < 1 ? null : JsonSerializer.Serialize(customNames);

    Content = content?.CleanTrim();
  }

  public void SetSize(ILineageSize size)
  {
    SizeCategory = size.Category;
    HeightRoll = size.Height;
  }

  public void SetSpeeds(ILineageSpeeds speeds)
  {
    Walk = speeds.Walk;
    Climb = speeds.Climb;
    Swim = speeds.Swim;
    Fly = speeds.Fly;
    Hover = speeds.Hover;
    Burrow = speeds.Burrow;
  }

  public void SetWeight(ILineageWeight weight)
  {
    Malnutrition = weight.Malnutrition;
    Skinny = weight.Skinny;
    NormalWeight = weight.Normal;
    Overweight = weight.Overweight;
    Obese = weight.Obese;
  }

  public void Update(Guid userId, DateTime? updatedOn = null)
  {
    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();
  }

  private static string? EncodeNames(IEnumerable<string> names)
  {
    IEnumerable<string> cleaned = CleanNames(names);
    return cleaned.Any() ? JsonSerializer.Serialize(cleaned) : null;
  }
  private static IEnumerable<string> CleanNames(IEnumerable<string> names) => names
    .Where(name => !string.IsNullOrWhiteSpace(name))
    .Select(name => name.Trim())
    .OrderBy(name => name)
    .Distinct();

  public override bool Equals(object? obj) => obj is Lineage lineage && lineage.LineageId == LineageId;
  public override int GetHashCode() => LineageId.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (LineageId={LineageId})";
}

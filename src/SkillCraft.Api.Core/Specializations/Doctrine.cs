using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Core.Specializations;

public class Doctrine
{
  public Name Name { get; }
  public IReadOnlyCollection<string> Description { get; }
  public IReadOnlyCollection<TalentId> DiscountedTalentIds { get; }
  public IReadOnlyCollection<Feature> Features { get; }

  [JsonIgnore]
  public long Size => Name.Size + Description.Sum(x => x.Length) + Features.Sum(x => x.Size);

  public Doctrine(Name name, IEnumerable<string> description, IEnumerable<Talent> discountedTalents, IEnumerable<Feature> features)
    : this(name, description.ToArray(), discountedTalents.Select(talent => talent.Id).ToArray(), features.ToArray())
  {
  }

  [JsonConstructor]
  public Doctrine(Name name, IReadOnlyCollection<string> description, IReadOnlyCollection<TalentId> discountedTalentIds, IReadOnlyCollection<Feature> features)
  {
    Name = name;
    Description = description.Where(description => !string.IsNullOrWhiteSpace(description)).Select(description => description.Trim()).Distinct().ToList().AsReadOnly();
    DiscountedTalentIds = discountedTalentIds.Distinct().OrderBy(id => id.Value).ToList().AsReadOnly();
    Features = features.GroupBy(x => x.Name).Select(x => x.Last()).OrderBy(x => x.Name.Value).ToList().AsReadOnly();
  }

  public override bool Equals(object? obj) => obj is Doctrine doctrine
    && doctrine.Name == Name
    && doctrine.Description.SequenceEqual(Description)
    && doctrine.DiscountedTalentIds.SequenceEqual(DiscountedTalentIds)
    && doctrine.Features.SequenceEqual(Features);
  public override int GetHashCode()
  {
    HashCode hash = new();
    hash.Add(Name);
    foreach (string description in Description)
    {
      hash.Add(description);
    }
    foreach (TalentId discountedTalentId in DiscountedTalentIds)
    {
      hash.Add(discountedTalentId);
    }
    foreach (Feature feature in Features)
    {
      hash.Add(feature);
    }
    return hash.ToHashCode();
  }
  public override string ToString() => string.Join(' ', GetType(), JsonSerializer.Serialize(this));
}

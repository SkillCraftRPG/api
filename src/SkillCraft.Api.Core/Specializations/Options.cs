using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Core.Specializations;

public class Options
{
  public IReadOnlyCollection<TalentId> TalentIds { get; } = [];
  public IReadOnlyCollection<string> Other { get; } = [];

  [JsonIgnore]
  public long Size => Other.Sum(other => other.Length);

  public Options()
  {
  }

  [JsonConstructor]
  public Options(IReadOnlyCollection<TalentId> talentIds, IReadOnlyCollection<string> other)
  {
    TalentIds = talentIds.Distinct().OrderBy(id => id.Value).ToList().AsReadOnly();
    Other = other.Where(requirement => !string.IsNullOrWhiteSpace(requirement)).Select(requirement => requirement.Trim()).Distinct().ToList().AsReadOnly();
  }

  public override bool Equals(object? obj) => obj is Options options
    && options.TalentIds.SequenceEqual(TalentIds)
    && options.Other.SequenceEqual(Other);
  public override int GetHashCode()
  {
    HashCode hash = new();
    foreach (TalentId talentId in TalentIds)
    {
      hash.Add(talentId);
    }
    foreach (string other in Other)
    {
      hash.Add(other);
    }
    return hash.ToHashCode();
  }
  public override string ToString() => string.Join(' ', GetType(), JsonSerializer.Serialize(this));
}

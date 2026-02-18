using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Core.Specializations;

public class Requirements
{
  public TalentId? TalentId { get; }
  public IReadOnlyCollection<string> Other { get; } = [];

  [JsonIgnore]
  public long Size => Other.Sum(other => other.Length);

  public Requirements()
  {
  }

  [JsonConstructor]
  public Requirements(TalentId? talentId, IReadOnlyCollection<string> other)
  {
    TalentId = talentId;
    Other = other.Where(requirement => !string.IsNullOrWhiteSpace(requirement)).Select(requirement => requirement.Trim()).Distinct().ToList().AsReadOnly();
  }

  public override bool Equals(object? obj) => obj is Requirements requirements
    && requirements.TalentId == TalentId
    && requirements.Other.SequenceEqual(Other);
  public override int GetHashCode()
  {
    HashCode hash = new();
    hash.Add(TalentId);
    foreach (string other in Other)
    {
      hash.Add(other.GetHashCode());
    }
    return hash.ToHashCode();
  }
  public override string ToString() => string.Join(' ', GetType(), JsonSerializer.Serialize(this));
}

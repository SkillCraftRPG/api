using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Core.Specializations;

public record Requirements
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
    Other = other.Where(requirement => !string.IsNullOrWhiteSpace(requirement)).Select(requirement => requirement.Trim()).ToList().AsReadOnly();
  }
}

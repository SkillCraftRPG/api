using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Core.Specializations;

public record Options
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
    TalentIds = talentIds.Distinct().ToList().AsReadOnly();
    Other = other.Where(requirement => !string.IsNullOrWhiteSpace(requirement)).Select(requirement => requirement.Trim()).Distinct().ToList().AsReadOnly();
  }
}

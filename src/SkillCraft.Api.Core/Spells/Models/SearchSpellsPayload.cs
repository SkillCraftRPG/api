using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Spells.Models;

public record SearchSpellsPayload : SearchPayload
{
  public List<int> Tiers { get; set; } = [];

  public new List<SpellSortOption> Sort { get; set; } = [];
}

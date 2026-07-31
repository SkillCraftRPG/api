using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Spells.Models;

public record SearchSpellsPayload : SearchPayload
{
  public new List<SpellSortOption> Sort { get; set; } = [];
}

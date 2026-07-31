using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Spells.Models;

namespace SkillCraft.Api.Models.Spell;

public record SearchSpellsParameters : SearchParameters
{
  public virtual SearchSpellsPayload ToPayload()
  {
    SearchSpellsPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out SpellSort field))
      {
        payload.Sort.Add(new SpellSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

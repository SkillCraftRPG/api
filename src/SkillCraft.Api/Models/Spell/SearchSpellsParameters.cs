using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Spells.Models;

namespace SkillCraft.Api.Models.Spell;

public record SearchSpellsParameters : SearchParameters
{
  [FromQuery(Name = "tier")]
  public List<int> Tiers { get; set; } = [];

  public virtual SearchSpellsPayload ToPayload()
  {
    SearchSpellsPayload payload = new();
    payload.Tiers.AddRange(Tiers);
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

using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Castes.Models;

namespace SkillCraft.Api.Models.Caste;

public record SearchCastesParameters : SearchParameters
{
  [FromQuery(Name = "skill")]
  public Skill? Skill { get; set; }

  public virtual SearchCastesPayload ToPayload()
  {
    SearchCastesPayload payload = new()
    {
      Skill = Skill
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out CasteSort field))
      {
        payload.Sort.Add(new CasteSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

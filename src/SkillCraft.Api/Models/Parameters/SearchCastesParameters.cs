using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Castes;

namespace SkillCraft.Api.Models.Parameters;

public record SearchCastesParameters : SearchParameters
{
  [FromQuery(Name = "skill")]
  public string? Skill { get; set; }

  public virtual SearchCastesPayload ToPayload()
  {
    SearchCastesPayload payload = new()
    {
      Skill = Skill
    };
    Fill(payload);

    foreach (SortOption item in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(item.Field, out CasteSort field))
      {
        payload.Sort.Add(new CasteSortOption(field, item.IsDescending));
      }
    }

    return payload;
  }
}

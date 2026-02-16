using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Talents;

namespace SkillCraft.Api.Models.Parameters;

public record SearchTalentsParameters : SearchParameters
{
  // TODO(fpion): Tier

  [FromQuery(Name = "multiple")]
  public bool? AllowMultiplePurchases { get; set; }

  // TODO(fpion): Skill

  // TODO(fpion): RequiredTalent

  public virtual SearchTalentsPayload ToPayload()
  {
    SearchTalentsPayload payload = new()
    {
      AllowMultiplePurchases = AllowMultiplePurchases
    };
    Fill(payload);

    foreach (SortOption item in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(item.Field, out TalentSort field))
      {
        payload.Sort.Add(new TalentSortOption(field, item.IsDescending));
      }
    }

    return payload;
  }
}

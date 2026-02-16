using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Talents;

namespace SkillCraft.Api.Models.Parameters;

public record SearchTalentsParameters : SearchParameters
{
  [FromQuery(Name = "tier")]
  public List<int> Tiers { get; set; } = [];

  [FromQuery(Name = "multiple")]
  public bool? AllowMultiplePurchases { get; set; }

  [FromQuery(Name = "skill")]
  public string? Skill { get; set; }

  [FromQuery(Name = "required")]
  public string? RequiredTalent { get; set; }

  public virtual SearchTalentsPayload ToPayload()
  {
    SearchTalentsPayload payload = new()
    {
      AllowMultiplePurchases = AllowMultiplePurchases,
      Skill = Skill,
      RequiredTalent = RequiredTalent
    };
    payload.Tiers.AddRange(Tiers);
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

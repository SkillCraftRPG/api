using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts;
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
      Skill = Skill
    };
    payload.Tiers.AddRange(Tiers);
    Fill(payload);

    if (!string.IsNullOrWhiteSpace(RequiredTalent))
    {
      string requiredTalent = RequiredTalent.Trim();
      if (requiredTalent.Equals("null", StringComparison.InvariantCultureIgnoreCase))
      {
        payload.RequiredTalent = new EntityFilter(null);
      }
      else if (Guid.TryParse(requiredTalent, out Guid id))
      {
        payload.RequiredTalent = new EntityFilter(id);
      }
    }

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

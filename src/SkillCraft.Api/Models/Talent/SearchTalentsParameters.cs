using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Talents.Models;

namespace SkillCraft.Api.Models.Talent;

public record SearchTalentsParameters : SearchParameters
{
  [FromQuery(Name = "tier")]
  public List<int> Tiers { get; set; } = [];

  [FromQuery(Name = "multiple")]
  public bool? AllowMultiplePurchases { get; set; }

  [FromQuery(Name = "skill")]
  public Skill? Skill { get; set; }

  [FromQuery(Name = "required")]
  public Guid? RequiredTalentId { get; set; }

  public virtual SearchTalentsPayload ToPayload()
  {
    SearchTalentsPayload payload = new()
    {
      AllowMultiplePurchases = AllowMultiplePurchases,
      Skill = Skill,
      RequiredTalentId = RequiredTalentId
    };
    payload.Tiers.AddRange(Tiers);
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out TalentSort field))
      {
        payload.Sort.Add(new TalentSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

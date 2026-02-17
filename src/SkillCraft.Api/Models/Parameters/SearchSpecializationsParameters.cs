using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Specializations;

namespace SkillCraft.Api.Models.Parameters;

public record SearchSpecializationsParameters : SearchParameters
{
  [FromQuery(Name = "tier")]
  public List<int> Tiers { get; set; } = [];

  [FromQuery(Name = "required")]
  public string? RequiredTalent { get; set; }

  public virtual SearchSpecializationsPayload ToPayload()
  {
    SearchSpecializationsPayload payload = new()
    {
      RequiredTalent = RequiredTalent
    };
    payload.Tiers.AddRange(Tiers);
    Fill(payload);

    foreach (SortOption item in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(item.Field, out SpecializationSort field))
      {
        payload.Sort.Add(new SpecializationSortOption(field, item.IsDescending));
      }
    }

    return payload;
  }
}

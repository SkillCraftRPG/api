using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Educations;

namespace SkillCraft.Api.Models.Parameters;

public record SearchEducationsParameters : SearchParameters
{
  [FromQuery(Name = "skill")]
  public string? Skill { get; set; }

  public virtual SearchEducationsPayload ToPayload()
  {
    SearchEducationsPayload payload = new()
    {
      Skill = Skill
    };
    Fill(payload);

    foreach (SortOption item in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(item.Field, out EducationSort field))
      {
        payload.Sort.Add(new EducationSortOption(field, item.IsDescending));
      }
    }

    return payload;
  }
}

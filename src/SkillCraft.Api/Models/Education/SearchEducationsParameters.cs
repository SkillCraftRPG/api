using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Educations.Models;

namespace SkillCraft.Api.Models.Education;

public record SearchEducationsParameters : SearchParameters
{
  [FromQuery(Name = "skill")]
  public Skill? Skill { get; set; }

  public virtual SearchEducationsPayload ToPayload()
  {
    SearchEducationsPayload payload = new()
    {
      Skill = Skill
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out EducationSort field))
      {
        payload.Sort.Add(new EducationSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

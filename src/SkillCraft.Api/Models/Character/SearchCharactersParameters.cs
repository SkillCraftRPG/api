using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Characters.Models;

namespace SkillCraft.Api.Models.Character;

public record SearchCharactersParameters : SearchParameters
{
  [FromQuery(Name = "lineage")]
  public Guid? LineageId { get; set; }

  [FromQuery(Name = "caste")]
  public Guid? CasteId { get; set; }

  [FromQuery(Name = "education")]
  public Guid? EducationId { get; set; }

  public virtual SearchCharactersPayload ToPayload()
  {
    SearchCharactersPayload payload = new()
    {
      LineageId = LineageId,
      CasteId = CasteId,
      EducationId = EducationId
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out CharacterSort field))
      {
        payload.Sort.Add(new CharacterSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

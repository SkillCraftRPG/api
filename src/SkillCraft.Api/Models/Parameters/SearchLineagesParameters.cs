using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Models.Parameters;

public record SearchLineagesParameters : SearchParameters
{
  [FromQuery(Name = "language")]
  public Guid? LanguageId { get; set; }

  [FromQuery(Name = "size")]
  public SizeCategory? SizeCategory { get; set; }

  public virtual SearchLineagesPayload ToPayload()
  {
    SearchLineagesPayload payload = new()
    {
      LanguageId = LanguageId,
      SizeCategory = SizeCategory
    };
    Fill(payload);

    foreach (SortOption item in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(item.Field, out LineageSort field))
      {
        payload.Sort.Add(new LineageSortOption(field, item.IsDescending));
      }
    }

    return payload;
  }
}

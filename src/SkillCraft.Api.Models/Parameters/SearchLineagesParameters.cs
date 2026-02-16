using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Models.Parameters;

public record SearchLineagesParameters : SearchParameters
{
  public virtual SearchLineagesPayload ToPayload()
  {
    SearchLineagesPayload payload = new();
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

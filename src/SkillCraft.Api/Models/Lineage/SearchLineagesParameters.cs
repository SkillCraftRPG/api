using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Lineages.Models;

namespace SkillCraft.Api.Models.Lineage;

public record SearchLineagesParameters : SearchParameters
{
  [FromQuery(Name = "parent")]
  public Guid? ParentId { get; set; }

  [FromQuery(Name = "size")]
  public SizeCategory? SizeCategory { get; set; }

  public virtual SearchLineagesPayload ToPayload()
  {
    SearchLineagesPayload payload = new()
    {
      ParentId = ParentId,
      SizeCategory = SizeCategory
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out LineageSort field))
      {
        payload.Sort.Add(new LineageSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

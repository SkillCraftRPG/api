using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Customizations;

namespace SkillCraft.Api.Models.Parameters;

public record SearchCustomizationsParameters : SearchParameters
{
  [FromQuery(Name = "kind")]
  public CustomizationKind? Kind { get; set; }

  public virtual SearchCustomizationsPayload ToPayload()
  {
    SearchCustomizationsPayload payload = new()
    {
      Kind = Kind
    };
    Fill(payload);

    foreach (SortOption item in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(item.Field, out CustomizationSort field))
      {
        payload.Sort.Add(new CustomizationSortOption(field, item.IsDescending));
      }
    }

    return payload;
  }
}

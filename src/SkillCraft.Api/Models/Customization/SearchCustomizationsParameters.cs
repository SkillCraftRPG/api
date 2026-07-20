using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Customizations.Models;

namespace SkillCraft.Api.Models.Customization;

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

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out CustomizationSort field))
      {
        payload.Sort.Add(new CustomizationSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Core.Items.Models;

namespace SkillCraft.Api.Models.Item;

public record SearchItemsParameters : SearchParameters
{
  [FromQuery(Name = "category")]
  public ItemCategory? Category { get; set; }

  [FromQuery(Name = "rarity")]
  public ItemRarity? Rarity { get; set; }

  [FromQuery(Name = "magic")]
  public bool? IsMagic { get; set; }

  public virtual SearchItemsPayload ToPayload()
  {
    SearchItemsPayload payload = new()
    {
      Category = Category,
      Rarity = Rarity,
      IsMagic = IsMagic
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out ItemSort field))
      {
        payload.Sort.Add(new ItemSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Items.Models;

namespace SkillCraft.Api.Models.Item;

public record SearchItemsParameters : SearchParameters
{
  public virtual SearchItemsPayload ToPayload()
  {
    SearchItemsPayload payload = new();
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

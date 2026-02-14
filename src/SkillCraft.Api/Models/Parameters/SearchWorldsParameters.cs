using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Worlds;

namespace SkillCraft.Api.Models.Parameters;

public record SearchWorldsParameters : SearchParameters
{
  public virtual SearchWorldsPayload ToPayload()
  {
    SearchWorldsPayload payload = new();
    Fill(payload);

    foreach (SortOption item in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(item.Field, out WorldSort field))
      {
        payload.Sort.Add(new WorldSortOption(field, item.IsDescending));
      }
    }

    return payload;
  }
}

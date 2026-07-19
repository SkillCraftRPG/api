using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Worlds.Models;

namespace SkillCraft.Api.Models.World;

public record SearchWorldsParameters : SearchParameters
{
  public virtual SearchWorldsPayload ToPayload()
  {
    SearchWorldsPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out WorldSort field))
      {
        payload.Sort.Add(new WorldSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

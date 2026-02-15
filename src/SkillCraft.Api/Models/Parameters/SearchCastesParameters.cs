using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Castes;

namespace SkillCraft.Api.Models.Parameters;

public record SearchCastesParameters : SearchParameters
{
  public virtual SearchCastesPayload ToPayload()
  {
    SearchCastesPayload payload = new();
    Fill(payload);

    foreach (SortOption item in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(item.Field, out CasteSort field))
      {
        payload.Sort.Add(new CasteSortOption(field, item.IsDescending));
      }
    }

    return payload;
  }
}

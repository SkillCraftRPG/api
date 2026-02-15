using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Parties;

namespace SkillCraft.Api.Models.Parameters;

public record SearchPartiesParameters : SearchParameters
{
  public virtual SearchPartiesPayload ToPayload()
  {
    SearchPartiesPayload payload = new();
    Fill(payload);

    foreach (SortOption item in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(item.Field, out PartySort field))
      {
        payload.Sort.Add(new PartySortOption(field, item.IsDescending));
      }
    }

    return payload;
  }
}

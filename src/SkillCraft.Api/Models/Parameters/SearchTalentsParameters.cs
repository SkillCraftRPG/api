using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Talents;

namespace SkillCraft.Api.Models.Parameters;

public record SearchTalentsParameters : SearchParameters
{
  public virtual SearchTalentsPayload ToPayload()
  {
    SearchTalentsPayload payload = new();
    Fill(payload);

    foreach (SortOption item in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(item.Field, out TalentSort field))
      {
        payload.Sort.Add(new TalentSortOption(field, item.IsDescending));
      }
    }

    return payload;
  }
}

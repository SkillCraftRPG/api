using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Languages;

namespace SkillCraft.Api.Models.Parameters;

public record SearchLanguagesParameters : SearchParameters
{
  public virtual SearchLanguagesPayload ToPayload()
  {
    SearchLanguagesPayload payload = new();
    Fill(payload);

    foreach (SortOption item in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(item.Field, out LanguageSort field))
      {
        payload.Sort.Add(new LanguageSortOption(field, item.IsDescending));
      }
    }

    return payload;
  }
}

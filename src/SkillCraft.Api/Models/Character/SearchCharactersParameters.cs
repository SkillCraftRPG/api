using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Characters.Models;

namespace SkillCraft.Api.Models.Character;

public record SearchCharactersParameters : SearchParameters
{
  public virtual SearchCharactersPayload ToPayload()
  {
    SearchCharactersPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out CharacterSort field))
      {
        payload.Sort.Add(new CharacterSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

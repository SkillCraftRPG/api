using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Scripts;

namespace SkillCraft.Api.Models.Parameters;

public record SearchScriptsParameters : SearchParameters
{
  public virtual SearchScriptsPayload ToPayload()
  {
    SearchScriptsPayload payload = new();
    Fill(payload);

    foreach (SortOption item in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(item.Field, out ScriptSort field))
      {
        payload.Sort.Add(new ScriptSortOption(field, item.IsDescending));
      }
    }

    return payload;
  }
}

using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Scripts.Models;

namespace SkillCraft.Api.Models.Script;

public record SearchScriptsParameters : SearchParameters
{
  public virtual SearchScriptsPayload ToPayload()
  {
    SearchScriptsPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out ScriptSort field))
      {
        payload.Sort.Add(new ScriptSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Languages;

namespace SkillCraft.Api.Models.Parameters;

public record SearchLanguagesParameters : SearchParameters
{
  [FromQuery(Name = "script")]
  public string? Script { get; set; }

  public virtual SearchLanguagesPayload ToPayload()
  {
    SearchLanguagesPayload payload = new();
    Fill(payload);

    if (!string.IsNullOrWhiteSpace(Script))
    {
      string script = Script.Trim();
      if (script.Equals("null", StringComparison.InvariantCultureIgnoreCase))
      {
        payload.Script = new ScriptFilter(Id: null);
      }
      else if (Guid.TryParse(script, out Guid id))
      {
        payload.Script = new ScriptFilter(id);
      }
    }

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

using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Languages.Models;

namespace SkillCraft.Api.Models.Language;

public record SearchLanguagesParameters : SearchParameters
{
  [FromQuery(Name = "script")]
  public Guid? ScriptId { get; set; }

  public virtual SearchLanguagesPayload ToPayload()
  {
    SearchLanguagesPayload payload = new()
    {
      ScriptId = ScriptId
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out LanguageSort field))
      {
        payload.Sort.Add(new LanguageSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

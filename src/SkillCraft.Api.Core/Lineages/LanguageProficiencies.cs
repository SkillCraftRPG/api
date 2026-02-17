using FluentValidation;
using SkillCraft.Api.Core.Languages;

namespace SkillCraft.Api.Core.Lineages;

public record LanguageProficiencies
{
  public IReadOnlyCollection<LanguageId> Ids { get; } = [];
  public int Extra { get; }
  public Description? Text { get; }

  [JsonIgnore]
  public long Size => Text?.Size ?? 0;

  public LanguageProficiencies()
  {
  }

  public LanguageProficiencies(IEnumerable<Language> languages, int extra, Description? text)
    : this(languages.Select(language => language.Id).ToArray(), extra, text)
  {
  }

  [JsonConstructor]
  public LanguageProficiencies(IReadOnlyCollection<LanguageId> ids, int extra, Description? text)
  {
    Ids = ids.Distinct().ToList().AsReadOnly();
    Extra = extra;
    Text = text;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<LanguageProficiencies>
  {
    public Validator()
    {
      RuleFor(x => x.Extra).GreaterThanOrEqualTo(0);
    }
  }
}

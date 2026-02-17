using FluentValidation;
using SkillCraft.Api.Core.Languages;

namespace SkillCraft.Api.Core.Lineages;

public record LineageLanguages // TODO(fpion): rename
{
  public IReadOnlyCollection<LanguageId> Ids { get; } = [];
  public int Extra { get; }
  public Description? Text { get; }

  [JsonIgnore]
  public long Size => Text?.Size ?? 0;

  public LineageLanguages()
  {
  }

  public LineageLanguages(IEnumerable<Language> languages, int extra, Description? text)
    : this(languages.Select(language => language.Id).ToArray(), extra, text)
  {
  }

  [JsonConstructor]
  public LineageLanguages(IReadOnlyCollection<LanguageId> ids, int extra, Description? text)
  {
    Ids = ids.Distinct().ToList().AsReadOnly();
    Extra = extra;
    Text = text;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<LineageLanguages>
  {
    public Validator()
    {
      RuleFor(x => x.Extra).GreaterThanOrEqualTo(0);
    }
  }
}

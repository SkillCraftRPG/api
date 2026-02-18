using FluentValidation;
using SkillCraft.Api.Core.Languages;

namespace SkillCraft.Api.Core.Lineages;

public class LanguageProficiencies
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
    Ids = ids.Distinct().OrderBy(id => id.Value).ToList().AsReadOnly();
    Extra = extra;
    Text = text;
    new Validator().ValidateAndThrow(this);
  }

  public override bool Equals(object? obj) => obj is LanguageProficiencies proficiencies
    && proficiencies.Ids.SequenceEqual(Ids)
    && proficiencies.Extra == Extra
    && proficiencies.Text == Text;
  public override int GetHashCode()
  {
    HashCode hash = new();
    foreach (LanguageId id in Ids)
    {
      hash.Add(id);
    }
    hash.Add(Extra);
    hash.Add(Text);
    return hash.ToHashCode();
  }
  public override string ToString() => string.Join(' ', GetType(), JsonSerializer.Serialize(this));

  private class Validator : AbstractValidator<LanguageProficiencies>
  {
    public Validator()
    {
      RuleFor(x => x.Extra).GreaterThanOrEqualTo(0);
    }
  }
}

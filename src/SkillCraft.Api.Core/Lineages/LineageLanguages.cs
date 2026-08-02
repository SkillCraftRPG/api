using FluentValidation;
using SkillCraft.Api.Core.Languages;

namespace SkillCraft.Api.Core.Lineages;

public class LineageLanguages
{
  public IReadOnlyCollection<LanguageId> Ids { get; } = [];
  public int Extra { get; }
  public Content? Content { get; }

  public LineageLanguages()
  {
  }

  [JsonConstructor]
  public LineageLanguages(IReadOnlyCollection<LanguageId> ids, int extra, Content? content)
  {
    Ids = ids.Distinct().ToList().AsReadOnly();
    Extra = extra;
    Content = content;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<LineageLanguages>
  {
    public Validator()
    {
      RuleFor(x => x.Extra).GreaterThanOrEqualTo(0);
    }
  }

  public override bool Equals(object? obj) => obj is LineageLanguages languages
    && languages.Ids.SequenceEqual(Ids)
    && languages.Extra == Extra
    && Equals(languages.Content, Content);
  public override int GetHashCode()
  {
    HashCode hash = new();
    foreach (LanguageId id in Ids)
    {
      hash.Add(id);
    }
    hash.Add(Extra);
    hash.Add(Content);
    return hash.ToHashCode();
  }
  public override string ToString()
  {
    StringBuilder value = new();
    value.AppendLine(base.ToString());

    value.Append(nameof(Ids)).Append(": ");
    if (Ids.Count < 1)
    {
      value.AppendLine("[]");
    }
    else
    {
      foreach (LanguageId id in Ids)
      {
        value.Append(" - ").Append(id).AppendLine();
      }
    }

    value.Append(nameof(Extra)).Append(": ").Append(Extra).AppendLine();
    value.Append(nameof(Content)).Append(": ").Append(Content is null ? "<null>" : Content).AppendLine();

    return value.ToString();
  }
}

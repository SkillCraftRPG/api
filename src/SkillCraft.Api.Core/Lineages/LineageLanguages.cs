using System.Text;

namespace SkillCraft.Api.Core.Lineages;

public class LineageLanguages
{
  public IReadOnlyCollection<Guid> Ids { get; }
  public int Extra { get; }
  public string? Content { get; }

  public LineageLanguages()
  {
    Ids = [];
  }

  [JsonConstructor]
  public LineageLanguages(IReadOnlyCollection<Guid> ids, int extra, string? content)
  {
    Ids = ids;
    Extra = extra;
    Content = content;
  }

  public LineageLanguages(Lineage lineage)
  {
    Ids = lineage.Languages.Select(language => language.Id).ToList().AsReadOnly();
    Extra = lineage.ExtraLanguages;
    Content = lineage.LanguagesContent;
  }

  public override bool Equals(object? obj) => obj is LineageLanguages languages
    && languages.Ids.SequenceEqual(Ids)
    && languages.Extra == Extra
    && languages.Content == Content;
  public override int GetHashCode()
  {
    HashCode hash = new();
    foreach (Guid id in Ids)
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
    if (Ids.Count > 0)
    {
      value.Append(nameof(Ids)).Append(':').AppendLine();
      foreach (Guid id in Ids)
      {
        value.Append(" - ").Append(id).AppendLine();
      }
    }
    value.Append(nameof(Extra)).Append(": ").Append(Extra).AppendLine();
    if (Content is not null)
    {
      value.Append(nameof(Content)).Append(": ").Append(Content).AppendLine();
    }
    return value.ToString();
  }
}

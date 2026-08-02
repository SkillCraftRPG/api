namespace SkillCraft.Api.Core.Lineages;

public class LineageNames
{
  public IReadOnlyCollection<string> Family { get; }
  public IReadOnlyCollection<string> Female { get; }
  public IReadOnlyCollection<string> Male { get; }
  public IReadOnlyCollection<string> Unisex { get; }
  public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Custom { get; }
  public Content? Content { get; }

  public LineageNames()
  {
    Family = [];
    Female = [];
    Male = [];
    Unisex = [];
    Custom = new Dictionary<string, IReadOnlyCollection<string>>().AsReadOnly();
  }

  [JsonConstructor]
  public LineageNames(
    IReadOnlyCollection<string> family,
    IReadOnlyCollection<string> female,
    IReadOnlyCollection<string> male,
    IReadOnlyCollection<string> unisex,
    IReadOnlyDictionary<string, IReadOnlyCollection<string>> custom,
    Content? content)
  {
    Family = Clean(family);
    Female = Clean(female);
    Male = Clean(male);
    Unisex = Clean(unisex);

    Dictionary<string, IReadOnlyCollection<string>> customNames = new(capacity: custom.Count);
    foreach (KeyValuePair<string, IReadOnlyCollection<string>> category in custom)
    {
      string key = category.Key.Trim();
      IReadOnlyCollection<string> names = Clean(category.Value);
      if (!string.IsNullOrEmpty(key) && names.Count > 0)
      {
        customNames[key] = names;
      }
    }
    Custom = customNames.AsReadOnly();

    Content = content;
  }

  private static IReadOnlyCollection<string> Clean(IEnumerable<string> names) => names
    .Where(name => !string.IsNullOrWhiteSpace(name))
    .Select(name => name.Trim())
    .OrderBy(name => name)
    .Distinct()
    .ToList()
    .AsReadOnly();

  public override bool Equals(object? obj) => obj is LineageNames names
    && names.Family.SequenceEqual(Family)
    && names.Female.SequenceEqual(Female)
    && names.Male.SequenceEqual(Male)
    && names.Unisex.SequenceEqual(Unisex)
    && AreEqual(names.Custom, Custom)
    && Equals(names.Content, Content);
  public override int GetHashCode()
  {
    HashCode hash = new();
    foreach (string name in Family)
    {
      hash.Add(name);
    }
    foreach (string name in Female)
    {
      hash.Add(name);
    }
    foreach (string name in Male)
    {
      hash.Add(name);
    }
    foreach (string name in Unisex)
    {
      hash.Add(name);
    }
    foreach (KeyValuePair<string, IReadOnlyCollection<string>> custom in Custom)
    {
      hash.Add(custom.Key);
      foreach (string name in custom.Value)
      {
        hash.Add(name);
      }
    }
    hash.Add(Content);
    return hash.ToHashCode();
  }
  public override string ToString()
  {
    StringBuilder value = new();
    value.AppendLine(base.ToString());

    value.Append(nameof(Family)).Append(": ").Append(Family.Count < 1 ? "[]" : string.Join(',', Family)).AppendLine();
    value.Append(nameof(Female)).Append(": ").Append(Female.Count < 1 ? "[]" : string.Join(',', Female)).AppendLine();
    value.Append(nameof(Male)).Append(": ").Append(Male.Count < 1 ? "[]" : string.Join(',', Male)).AppendLine();
    value.Append(nameof(Unisex)).Append(": ").Append(Unisex.Count < 1 ? "[]" : string.Join(',', Unisex)).AppendLine();

    foreach (KeyValuePair<string, IReadOnlyCollection<string>> custom in Custom)
    {
      value.Append(custom.Key).Append(": ").Append(string.Join(',', custom.Value)).AppendLine();
    }

    value.Append(nameof(Content)).Append(": ").Append(Content is null ? "<null>" : Content).AppendLine();

    return value.ToString();
  }

  private static bool AreEqual(IReadOnlyDictionary<string, IReadOnlyCollection<string>> left, IReadOnlyDictionary<string, IReadOnlyCollection<string>> right)
  {
    if (left.Count != right.Count)
    {
      return false;
    }

    foreach (KeyValuePair<string, IReadOnlyCollection<string>> category in left)
    {
      if (!right.TryGetValue(category.Key, out IReadOnlyCollection<string>? names) || !category.Value.SequenceEqual(names))
      {
        return false;
      }
    }

    return true;
  }
}

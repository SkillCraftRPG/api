namespace SkillCraft.Api.Core.Lineages;

public class LineageNames
{
  public IReadOnlyCollection<string> Family { get; }
  public IReadOnlyCollection<string> Female { get; }
  public IReadOnlyCollection<string> Male { get; }
  public IReadOnlyCollection<string> Unisex { get; }
  public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Custom { get; }
  public string? Content { get; }

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
    string? content)
  {
    Family = family;
    Female = female;
    Male = male;
    Unisex = unisex;
    Custom = custom;
    Content = content;
  }

  public LineageNames(Lineage lineage)
  {
    Family = Decode(lineage.FamilyNames);
    Female = Decode(lineage.FemaleNames);
    Male = Decode(lineage.MaleNames);
    Unisex = Decode(lineage.UnisexNames);
    Custom = DecodeCustom(lineage.CustomNames);
    Content = lineage.NamesContent;
  }

  public static IReadOnlyCollection<string> Decode(string? names)
  {
    return (names is null ? null : JsonSerializer.Deserialize<IReadOnlyCollection<string>>(names)) ?? [];
  }

  public static IReadOnlyDictionary<string, IReadOnlyCollection<string>> DecodeCustom(string? custom)
  {
    return (custom is null ? null : JsonSerializer.Deserialize<IReadOnlyDictionary<string, IReadOnlyCollection<string>>>(custom))
      ?? new Dictionary<string, IReadOnlyCollection<string>>().AsReadOnly();
  }

  public override bool Equals(object? obj) => obj is LineageNames names
    && names.Family.SequenceEqual(Family)
    && names.Female.SequenceEqual(Female)
    && names.Male.SequenceEqual(Male)
    && names.Unisex.SequenceEqual(Unisex)
    && AreEqual(names.Custom, Custom)
    && names.Content == Content;
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
    if (Family.Count > 0)
    {
      value.Append(nameof(Family)).Append(':').Append(string.Join(',', Family)).AppendLine();
    }
    if (Female.Count > 0)
    {
      value.Append(nameof(Female)).Append(':').Append(string.Join(',', Female)).AppendLine();
    }
    if (Male.Count > 0)
    {
      value.Append(nameof(Male)).Append(':').Append(string.Join(',', Male)).AppendLine();
    }
    if (Unisex.Count > 0)
    {
      value.Append(nameof(Unisex)).Append(':').Append(string.Join(',', Unisex)).AppendLine();
    }
    foreach (KeyValuePair<string, IReadOnlyCollection<string>> custom in Custom)
    {
      value.Append(custom.Key).Append(':').Append(string.Join(',', custom.Value)).AppendLine();
    }
    if (Content is not null)
    {
      value.Append(nameof(Content)).Append(':').Append(Content).AppendLine();
    }
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

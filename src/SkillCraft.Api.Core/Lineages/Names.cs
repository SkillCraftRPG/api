namespace SkillCraft.Api.Core.Lineages;

public class Names
{
  public IReadOnlyCollection<string> Family { get; }
  public IReadOnlyCollection<string> Female { get; }
  public IReadOnlyCollection<string> Male { get; }
  public IReadOnlyCollection<string> Unisex { get; }
  public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Custom { get; }
  public Description? Text { get; }

  [JsonIgnore]
  public long Size
  {
    get
    {
      long size = Family.Sum(name => name.Length) + Female.Sum(name => name.Length) + Male.Sum(name => name.Length) + Unisex.Sum(name => name.Length);
      foreach (KeyValuePair<string, IReadOnlyCollection<string>> nameCategory in Custom)
      {
        size += nameCategory.Key.Length;
        foreach (string name in nameCategory.Value)
        {
          size += name.Length;
        }
      }
      if (Text is not null)
      {
        size += Text.Size;
      }
      return size;
    }
  }

  public Names() : this([], [], [], [], new Dictionary<string, IReadOnlyCollection<string>>())
  {
  }

  [JsonConstructor]
  public Names(
    IReadOnlyCollection<string> family,
    IReadOnlyCollection<string> female,
    IReadOnlyCollection<string> male,
    IReadOnlyCollection<string> unisex,
    IReadOnlyDictionary<string, IReadOnlyCollection<string>> custom,
    Description? text = null)
  {
    Family = Sanitize(family);
    Female = Sanitize(female);
    Male = Sanitize(male);
    Unisex = Sanitize(unisex);

    Dictionary<string, IReadOnlyCollection<string>> customSanitized = new(capacity: custom.Count);
    foreach (KeyValuePair<string, IReadOnlyCollection<string>> pair in custom)
    {
      string category = pair.Key.Trim();
      IReadOnlyCollection<string> names = Sanitize(pair.Value);

      if (category.Length < 1)
      {
        throw new ArgumentException("The category key is required.", nameof(custom));
      }
      else if (category.Length > Name.MaximumLength)
      {
        throw new ArgumentException($"The category name '{pair.Key}' must not exceed {Name.MaximumLength} characters.", nameof(custom));
      }
      else if (names.Count > 0)
      {
        customSanitized[category] = names;
      }
    }
    Custom = customSanitized.AsReadOnly();

    Text = text;
  }

  public override bool Equals(object? obj) => obj is Names names
    && names.Family.SequenceEqual(Family)
    && names.Female.SequenceEqual(Female)
    && names.Male.SequenceEqual(Male)
    && names.Unisex.SequenceEqual(Unisex)
    && CustomEquals(names.Custom)
    && names.Text == Text;
  public override int GetHashCode()
  {
    HashCode hash = new();
    foreach (string family in Family)
    {
      hash.Add(family);
    }
    foreach (string female in Female)
    {
      hash.Add(female);
    }
    foreach (string male in Male)
    {
      hash.Add(male);
    }
    foreach (string unisex in Unisex)
    {
      hash.Add(unisex);
    }
    foreach (KeyValuePair<string, IReadOnlyCollection<string>> custom in Custom.OrderBy(x => x.Key))
    {
      hash.Add(custom.Key);
      foreach (string name in custom.Value)
      {
        hash.Add(name);
      }
    }
    hash.Add(Text);
    return hash.ToHashCode();
  }
  public override string ToString() => string.Join(' ', GetType(), JsonSerializer.Serialize(this));

  private bool CustomEquals(IReadOnlyDictionary<string, IReadOnlyCollection<string>> custom) => Custom.Count == custom.Count
    && Custom.All(pair => custom.TryGetValue(pair.Key, out IReadOnlyCollection<string>? names) && pair.Value.SequenceEqual(names));

  private static IReadOnlyCollection<string> Sanitize(IEnumerable<string> names) => names
    .Where(name => !string.IsNullOrWhiteSpace(name))
    .Select(name => name.Trim())
    .Distinct()
    .OrderBy(name => name)
    .ToList()
    .AsReadOnly();
}

namespace SkillCraft.Api.Core.Lineages;

public record Names
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

  private static IReadOnlyCollection<string> Sanitize(IEnumerable<string> names) => names
    .Where(name => !string.IsNullOrWhiteSpace(name))
    .Select(name => name.Trim())
    .Distinct()
    .OrderBy(name => name)
    .ToList()
    .AsReadOnly();
}

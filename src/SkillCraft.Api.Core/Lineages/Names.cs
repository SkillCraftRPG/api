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
  public long Size => Family.Sum(name => name.Length) + Female.Sum(name => name.Length) + Male.Sum(name => name.Length) + Unisex.Sum(name => name.Length) + (Text?.Size ?? 0); // TODO(fpion): Custom

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
    foreach (KeyValuePair<string, IReadOnlyCollection<string>> category in custom)
    {
      if (string.IsNullOrWhiteSpace(category.Key))
      {
        throw new ArgumentException("The category key is required.", nameof(custom));
      }

      IReadOnlyCollection<string> names = Sanitize(category.Value);
      if (names.Count > 0)
      {
        customSanitized[category.Key.Trim()] = names;
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

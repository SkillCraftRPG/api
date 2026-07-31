namespace SkillCraft.Api.Core.Lineages;

public class LineageNames
{
  public IReadOnlyCollection<string> Family { get; }
  public IReadOnlyCollection<string> Female { get; }
  public IReadOnlyCollection<string> Male { get; }
  public IReadOnlyCollection<string> Unisex { get; }
  public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Custom { get; }
  public string? Content { get; }

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
}

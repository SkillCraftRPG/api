using SkillCraft.Api.Core.Lineages.Events;

namespace SkillCraft.Api.Core.Lineages;

public record LineageSnapshot
{
  public string Name { get; }
  public string? Summary { get; }
  public string? Content { get; }

  public LineageLanguages Languages { get; }
  public LineageNames Names { get; }
  public LineageSpeeds Speeds { get; }
  public LineageSize Size { get; }
  public LineageWeight Weight { get; }
  public LineageAge Age { get; }

  public LineageSnapshot(Lineage lineage)
  {
    Name = lineage.Name;
    Summary = lineage.Summary;
    Content = lineage.Content;

    Languages = new LineageLanguages(lineage);
    Names = new LineageNames(lineage);
    Speeds = new LineageSpeeds(lineage);
    Size = new LineageSize(lineage);
    Weight = new LineageWeight(lineage);
    Age = new LineageAge(lineage);
  }

  public LineageUpdated? Compare(Lineage lineage)
  {
    int changes = 0;
    LineageUpdated record = new(lineage);

    if (Name != lineage.Name)
    {
      changes++;
      record.Name = new Change<string>(Name, lineage.Name);
    }

    if (Summary != lineage.Summary)
    {
      changes++;
      record.Summary = new Change<string>(Summary, lineage.Summary);
    }

    if (Content != lineage.Content)
    {
      changes++;
      record.Content = new Change<string>(Content, lineage.Content);
    }

    LineageLanguages languages = new(lineage);
    if (!Languages.Equals(languages))
    {
      changes++;
      record.Languages = new Change<LineageLanguages>(Languages, languages);
    }

    LineageNames names = new(lineage);
    if (!Names.Equals(names))
    {
      changes++;
      record.Names = new Change<LineageNames>(Names, names);
    }

    LineageSpeeds speeds = new(lineage);
    if (Speeds != speeds)
    {
      changes++;
      record.Speeds = new Change<LineageSpeeds>(Speeds, speeds);
    }

    LineageSize size = new(lineage);
    if (Size != size)
    {
      changes++;
      record.Size = new Change<LineageSize>(Size, size);
    }

    LineageWeight weight = new(lineage);
    if (Weight != weight)
    {
      changes++;
      record.Weight = new Change<LineageWeight>(Weight, weight);
    }

    LineageAge age = new(lineage);
    if (Age != age)
    {
      changes++;
      record.Age = new Change<LineageAge>(Age, age);
    }

    return changes < 1 ? null : record;
  }
}

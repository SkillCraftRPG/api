using Krakenar.Contracts;
using SkillCraft.Api.Core;

namespace SkillCraft.Api.Infrastructure.Compendium.Models;

internal class SpeciesEntry : Aggregate
{
  public string Slug { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;

  public SpeciesLanguagesEntry Languages { get; set; } = new();
  public SpeciesNamesEntry Names { get; set; } = new();
  public SpeciesSpeedsEntry Speeds { get; set; } = new();

  public string? MetaDescription { get; set; }
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public List<FeatureEntry> Features { get; set; } = [];

  public SpeciesCategoryEntry Category { get; set; } = new();
  public SpeciesSizeEntry Size { get; set; } = new();
  public SpeciesWeightEntry Weight { get; set; } = new();
  public SpeciesAgeEntry Age { get; set; } = new();

  public List<EthnicityEntry> Ethnicities { get; set; } = [];

  public override string ToString() => $"{Name} | {base.ToString()}";
}

internal class EthnicityEntry : Aggregate
{
  public string Slug { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;

  public SpeciesLanguagesEntry Languages { get; set; } = new();
  public SpeciesNamesEntry Names { get; set; } = new();
  public SpeciesSpeedsEntry Speeds { get; set; } = new();

  public string? MetaDescription { get; set; }
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public List<FeatureEntry> Features { get; set; } = [];

  public override string ToString() => $"{Name} | {base.ToString()}";
}

internal class SpeciesCategoryEntry : Aggregate
{
  public string Key { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;

  public int Order { get; set; }
  public int Columns { get; set; }

  public string? HtmlContent { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}

internal record SpeciesLanguagesEntry
{
  public List<LanguageEntry> Items { get; set; } = [];
  public int Extra { get; set; }
  public string? Text { get; set; }
}

internal record SpeciesNamesEntry
{
  public List<string> Family { get; set; } = [];
  public List<string> Female { get; set; } = [];
  public List<string> Male { get; set; } = [];
  public List<string> Unisex { get; set; } = [];
  public List<SpeciesNameCategoryEntry> Custom { get; set; } = [];
  public string? Text { get; set; }
}

internal record SpeciesNameCategoryEntry
{
  public string Category { get; set; } = string.Empty;
  public List<string> Values { get; set; } = [];
}

internal record SpeciesSpeedsEntry
{
  public int Walk { get; set; }
  public int Climb { get; set; }
  public int Swim { get; set; }
  public int Fly { get; set; }
  public bool Hover { get; set; }
  public int Burrow { get; set; }
}

internal record SpeciesSizeEntry
{
  public SizeCategory Category { get; set; }
  public string? Roll { get; set; }
}

internal record SpeciesWeightEntry
{
  public string? Malnutrition { get; set; }
  public string? Skinny { get; set; }
  public string? Normal { get; set; }
  public string? Overweight { get; set; }
  public string? Obese { get; set; }
}

internal record SpeciesAgeEntry
{
  public int Teenager { get; set; }
  public int Adult { get; set; }
  public int Mature { get; set; }
  public int Venerable { get; set; }
}

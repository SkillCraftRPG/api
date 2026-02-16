using Krakenar.Contracts;

namespace SkillCraft.Api.Contracts.Languages;

public class LanguageModel : Aggregate
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}

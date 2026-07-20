using Krakenar.Contracts;
using SkillCraft.Api.Core.Scripts.Models;

namespace SkillCraft.Api.Core.Languages.Models;

public class LanguageModel : Aggregate
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }

  public ScriptModel? Script { get; set; }
  public string? TypicalSpeakers { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}

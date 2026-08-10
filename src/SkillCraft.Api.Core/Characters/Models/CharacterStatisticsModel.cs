namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterStatisticsModel
{
  public CharacterStatisticModel Dodge { get; set; } = new();
  public CharacterStatisticModel Initiative { get; set; } = new();
  public CharacterStatisticModel Learning { get; set; } = new();
  public CharacterStatisticModel Load { get; set; } = new();
  public CharacterStatisticModel Power { get; set; } = new();
  public CharacterStatisticModel Precision { get; set; } = new();
  public CharacterStatisticModel Stamina { get; set; } = new();
  public CharacterStatisticModel Stratagem { get; set; } = new();
  public CharacterStatisticModel Strength { get; set; } = new();
  public CharacterStatisticModel Vitality { get; set; } = new();
}

namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterSkillsModel
{
  public CharacterSkillModel Acrobatics { get; set; } = new();
  public CharacterSkillModel Athletics { get; set; } = new();
  public CharacterSkillModel Crafting { get; set; } = new();
  public CharacterSkillModel Deception { get; set; } = new();
  public CharacterSkillModel Diplomacy { get; set; } = new();
  public CharacterSkillModel Discipline { get; set; } = new();
  public CharacterSkillModel Insight { get; set; } = new();
  public CharacterSkillModel Investigation { get; set; } = new();
  public CharacterSkillModel Knowledge { get; set; } = new();
  public CharacterSkillModel Linguistics { get; set; } = new();
  public CharacterSkillModel Medicine { get; set; } = new();
  public CharacterSkillModel Melee { get; set; } = new();
  public CharacterSkillModel Occultism { get; set; } = new();
  public CharacterSkillModel Orientation { get; set; } = new();
  public CharacterSkillModel Perception { get; set; } = new();
  public CharacterSkillModel Performance { get; set; } = new();
  public CharacterSkillModel Resistance { get; set; } = new();
  public CharacterSkillModel Stealth { get; set; } = new();
  public CharacterSkillModel Survival { get; set; } = new();
  public CharacterSkillModel Thievery { get; set; } = new();

  public int PointsSpent => Acrobatics.Rank + Athletics.Rank + Crafting.Rank + Deception.Rank + Diplomacy.Rank + Discipline.Rank
    + Insight.Rank + Investigation.Rank + Knowledge.Rank + Linguistics.Rank + Medicine.Rank + Melee.Rank + Occultism.Rank
    + Orientation.Rank + Perception.Rank + Performance.Rank + Resistance.Rank + Stealth.Rank + Survival.Rank + Thievery.Rank;
}

using Krakenar.Contracts;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Core.Educations.Models;
using SkillCraft.Api.Core.Lineages.Models;

namespace SkillCraft.Api.Core.Characters.Models;

public class CharacterModel : Aggregate
{
  public string Name { get; set; } = string.Empty;
  public DominantHand? DominantHand { get; set; }

  public int Tier { get; set; }
  public int Level { get; set; }
  public int Experience { get; set; }

  public LineageModel Lineage { get; set; } = new();
  public CasteModel Caste { get; set; } = new();
  public EducationModel Education { get; set; } = new();

  public CharacterAppearanceModel Appearance { get; set; } = new();
  public Alignment? Alignment { get; set; }
  public CharacterPersonalityModel Personality { get; set; } = new();
  public string? Background { get; set; }

  public CharacterAttributesModel Attributes { get; set; } = new();
  public CharacterStatisticsModel Statistics { get; set; } = new();
  public CharacterSkillsModel Skills { get; set; } = new();
  public CharacterSpeedsModel Speeds { get; set; } = new();

  public int Vitality { get; set; }
  public int Stamina { get; set; }
  public int BloodAlcoholContent { get; set; }
  public int Intoxication { get; set; }
  public int Hope { get; set; } // TODO(fpion): max. Hope?

  public List<CustomizationModel> Customizations { get; set; } = [];
  public List<CharacterLanguageModel> Languages { get; set; } = [];
  public List<CharacterModifierModel> Modifiers { get; set; } = [];
  public List<CharacterTalentModel> Talents { get; set; } = [];

  public CharacterPointsModel Points { get; set; } = new();

  /* TODO(fpion): complete this
   * Player
   * Picture
   * Inventory & Load
   * Attacks & Defense
   * Notes
   * Conditions
   * Specializations
   * Spells
   */
}

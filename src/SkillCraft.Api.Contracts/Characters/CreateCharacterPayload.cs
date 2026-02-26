namespace SkillCraft.Api.Contracts.Characters;

public record CreateCharacterPayload
{
  public string Name { get; set; }

  public Guid LineageId { get; set; }
  public Guid CasteId { get; set; }
  public Guid EducationId { get; set; }

  public CharacteristicsModel Characteristics { get; set; } = new();
  public StartingAttributesPayload StartingAttributes { get; set; } = new();

  public List<Guid> CustomizationIds { get; set; } = [];
  public List<Guid> LanguageIds { get; set; } = [];

  // TODO(fpion): Talents & Spells
  // TODO(fpion): Skill Ranks
  // TODO(fpion): Starting Wealth & Inventory (Attacks, Weapons, Shields, Armor, etc.)
  // TODO(fpion): Bonuses
  // TODO(fpion): Picture URL
  // TODO(fpion): Token URL

  public CreateCharacterPayload() : this(string.Empty)
  {
  }

  public CreateCharacterPayload(string name)
  {
    Name = name;
  }
}

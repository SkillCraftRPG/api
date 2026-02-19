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

  // TODO(fpion): Skill Ranks
  // TODO(fpion): Bonuses
  // TODO(fpion): Starting Wealth
  // TODO(fpion): Inventory (Attacks, Weapons, Shields, Armor, etc.)
  // TODO(fpion): Talents

  public CreateCharacterPayload() : this(string.Empty)
  {
  }

  public CreateCharacterPayload(string name)
  {
    Name = name;
  }
}

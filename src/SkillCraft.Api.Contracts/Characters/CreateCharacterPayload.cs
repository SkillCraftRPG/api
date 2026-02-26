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
  public List<Guid> TalentIds { get; set; } = [];

  public CreateCharacterPayload() : this(string.Empty)
  {
  }

  public CreateCharacterPayload(string name)
  {
    Name = name;
  }
}

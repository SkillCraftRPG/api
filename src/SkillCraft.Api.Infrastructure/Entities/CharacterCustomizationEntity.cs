namespace SkillCraft.Api.Infrastructure.Entities;

internal class CharacterCustomizationEntity
{
  public CharacterEntity? Character { get; private set; }
  public int CharacterId { get; private set; }

  public CustomizationEntity? Customization { get; private set; }
  public int CustomizationId { get; private set; }

  public CharacterCustomizationEntity(CharacterEntity character, CustomizationEntity customization)
  {
    Character = character;
    CharacterId = character.CharacterId;

    Customization = customization;
    CustomizationId = customization.CustomizationId;
  }

  private CharacterCustomizationEntity()
  {
  }

  public override bool Equals(object? obj) => obj is CharacterCustomizationEntity entity && entity.CharacterId == CharacterId && entity.CustomizationId == CustomizationId;
  public override int GetHashCode() => HashCode.Combine(CharacterId, CustomizationId);
  public override string ToString() => $"{base.ToString()} (CharacterId={CharacterId}, CustomizationId={CustomizationId})";
}

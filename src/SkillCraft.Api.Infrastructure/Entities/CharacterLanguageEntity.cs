namespace SkillCraft.Api.Infrastructure.Entities;

internal class CharacterLanguageEntity
{
  public int CharacterId { get; private set; }
  public int LanguageId { get; private set; }

  private CharacterLanguageEntity()
  {
  }

  public override bool Equals(object? obj) => obj is CharacterLanguageEntity entity && entity.CharacterId == CharacterId && entity.LanguageId == LanguageId;
  public override int GetHashCode() => HashCode.Combine(CharacterId, LanguageId);
  public override string ToString() => $"{base.ToString()} (CharacterId={CharacterId}, LanguageId={LanguageId})";
}

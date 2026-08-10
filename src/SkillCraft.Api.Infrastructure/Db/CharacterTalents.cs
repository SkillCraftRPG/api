using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class CharacterTalents
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.CharacterTalents), alias: null);

  public static readonly ColumnId CharacterId = new(nameof(CharacterTalentEntity.CharacterId), Table);
  public static readonly ColumnId Discounts = new(nameof(CharacterTalentEntity.Discounts), Table);
  public static readonly ColumnId Id = new(nameof(CharacterTalentEntity.Id), Table);
  public static readonly ColumnId Notes = new(nameof(CharacterTalentEntity.Notes), Table);
  public static readonly ColumnId Qualifier = new(nameof(CharacterTalentEntity.Qualifier), Table);
  public static readonly ColumnId TalentId = new(nameof(CharacterTalentEntity.TalentId), Table);
}

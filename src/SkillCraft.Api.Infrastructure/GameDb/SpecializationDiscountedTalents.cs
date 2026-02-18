using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.GameDb;

internal static class SpecializationDiscountedTalents
{
  public static readonly TableId Table = new(GameContext.Schema, nameof(GameContext.SpecializationDiscountedTalents), alias: null);

  public static readonly ColumnId SpecializationId = new(nameof(SpecializationDiscountedTalentEntity.SpecializationId), Table);
  public static readonly ColumnId SpecializationUid = new(nameof(SpecializationDiscountedTalentEntity.SpecializationUid), Table);
  public static readonly ColumnId TalentId = new(nameof(SpecializationDiscountedTalentEntity.TalentId), Table);
  public static readonly ColumnId TalentUid = new(nameof(SpecializationDiscountedTalentEntity.TalentUid), Table);
}

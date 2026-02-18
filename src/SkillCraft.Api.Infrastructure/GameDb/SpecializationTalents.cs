using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.GameDb;

internal static class SpecializationTalents
{
  public static readonly TableId Table = new(GameContext.Schema, nameof(GameContext.SpecializationOptionalTalents), alias: null);

  public static readonly ColumnId SpecializationId = new(nameof(SpecializationOptionalTalentEntity.SpecializationId), Table);
  public static readonly ColumnId SpecializationUid = new(nameof(SpecializationOptionalTalentEntity.SpecializationUid), Table);
  public static readonly ColumnId TalentId = new(nameof(SpecializationOptionalTalentEntity.TalentId), Table);
  public static readonly ColumnId TalentUid = new(nameof(SpecializationOptionalTalentEntity.TalentUid), Table);
}

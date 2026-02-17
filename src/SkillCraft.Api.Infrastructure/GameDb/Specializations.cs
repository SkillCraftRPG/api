using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.GameDb;

internal static class Specializations
{
  public static readonly TableId Table = new(GameContext.Schema, nameof(GameContext.Specializations), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(SpecializationEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(SpecializationEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(SpecializationEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(SpecializationEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(SpecializationEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(SpecializationEntity.Version), Table);

  public static readonly ColumnId Description = new(nameof(SpecializationEntity.Description), Table);
  public static readonly ColumnId Id = new(nameof(SpecializationEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(SpecializationEntity.Name), Table);
  public static readonly ColumnId OtherRequirements = new(nameof(SpecializationEntity.OtherRequirements), Table);
  public static readonly ColumnId RequiredTalentId = new(nameof(SpecializationEntity.RequiredTalentId), Table);
  public static readonly ColumnId RequiredTalentUid = new(nameof(SpecializationEntity.RequiredTalentUid), Table);
  public static readonly ColumnId SpecializationId = new(nameof(SpecializationEntity.SpecializationId), Table);
  public static readonly ColumnId Summary = new(nameof(SpecializationEntity.Summary), Table);
  public static readonly ColumnId Tier = new(nameof(SpecializationEntity.Tier), Table);
  public static readonly ColumnId WorldId = new(nameof(SpecializationEntity.WorldId), Table);
  public static readonly ColumnId WorldUid = new(nameof(SpecializationEntity.WorldUid), Table);

  // TODO(fpion): Options { Talents, Other }
  // TODO(fpion): Doctrine { Name, Description, DiscountedTalents, Features }
}

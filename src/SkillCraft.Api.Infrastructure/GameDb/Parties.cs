using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.GameDb;

internal static class Parties
{
  public static readonly TableId Table = new(GameContext.Schema, nameof(GameContext.Parties), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(PartyEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(PartyEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(PartyEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(PartyEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(PartyEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(PartyEntity.Version), Table);

  public static readonly ColumnId Description = new(nameof(PartyEntity.Description), Table);
  public static readonly ColumnId Id = new(nameof(PartyEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(PartyEntity.Name), Table);
  public static readonly ColumnId PartyId = new(nameof(PartyEntity.PartyId), Table);
  public static readonly ColumnId WorldId = new(nameof(PartyEntity.WorldId), Table);
  public static readonly ColumnId WorldUid = new(nameof(PartyEntity.WorldUid), Table);
}

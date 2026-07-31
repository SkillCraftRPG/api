using Logitar.Data;

namespace SkillCraft.Api.Infrastructure.Db;

public static class LineageLanguages
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.LineageLanguages), alias: null);
}

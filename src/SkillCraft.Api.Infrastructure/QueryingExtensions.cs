using Krakenar.Contracts.Search;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure;

internal static class QueryingExtensions
{
  public static IQueryBuilder ApplyIdFilter(this IQueryBuilder builder, ColumnId column, IEnumerable<Guid> ids)
  {
    if (!ids.Any())
    {
      return builder;
    }

    object[] values = ids.Distinct().Select(id => (object)id).ToArray();
    return builder.Where(column, Operators.IsIn(values));
  }

  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, SearchPayload payload)
  {
    return query.ApplyPaging(payload.Skip, payload.Limit);
  }
  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int skip, int limit)
  {
    if (skip > 0)
    {
      query = query.Skip(skip);
    }
    if (limit > 0)
    {
      query = query.Take(limit);
    }
    return query;
  }

  public static IQueryable<T> FromQuery<T>(this DbSet<T> entities, IQueryBuilder builder) where T : class
  {
    return entities.FromQuery(builder.Build());
  }
  public static IQueryable<T> FromQuery<T>(this DbSet<T> entities, IQuery query) where T : class
  {
    return entities.FromSqlRaw(query.Text, query.Parameters.ToArray());
  }

  public static IQueryable<T> WhereWorld<T>(this IQueryable<T> query, WorldId worldId) where T : IWorldScoped
  {
    return query.WhereWorld(worldId.ToGuid());
  }
  public static IQueryable<T> WhereWorld<T>(this IQueryable<T> query, Guid worldId) where T : IWorldScoped
  {
    return query.Where(x => x.WorldUid == worldId);
  }
}

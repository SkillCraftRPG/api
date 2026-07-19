using Logitar.Data;
using Logitar.Data.PostgreSQL;
using SkillCraft.Api.Infrastructure;

namespace SkillCraft.Api.PostgreSQL;

public class PostgresHelper : SqlHelper
{
  public override IQueryBuilder Query(TableId table) => PostgresQueryBuilder.From(table);

  protected override ConditionalOperator CreateOperator(string pattern) => PostgresOperators.IsLikeInsensitive(pattern);
}

namespace SkillCraft.Api.Core;

internal class QueryBus : Logitar.CQRS.QueryBus
{
  public QueryBus(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
}

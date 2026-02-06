using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Infrastructure;

namespace SkillCraft.Api.PostgreSQL;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApiPostgreSQL(this IServiceCollection services, string connectionString)
  {
    return services
      .AddLogitarEventSourcingWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddDbContext<GameContext>(options => options.UseNpgsql(connectionString, options => options.MigrationsAssembly("SkillCraft.Api.PostgreSQL")))
      .AddSingleton<ISqlHelper, PostgresHelper>();
  }
}

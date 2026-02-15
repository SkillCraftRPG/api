using Logitar;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Infrastructure;

namespace SkillCraft.Api.PostgreSQL;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApiPostgreSQL(this IServiceCollection services, IConfiguration configuration)
  {
    string? connectionString = EnvironmentHelper.TryGetString("POSTGRESQLCONNSTR_Krakenar") ?? configuration.GetConnectionString("PostgreSQL");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new InvalidOperationException("The PostgreSQL connection string was not found.");
    }
    return services.AddSkillCraftApiPostgreSQL(connectionString);
  }
  public static IServiceCollection AddSkillCraftApiPostgreSQL(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<GameContext>(options => options.UseNpgsql(connectionString, options => options.MigrationsAssembly("SkillCraft.Api.PostgreSQL")))
      .AddLogitarEventSourcingWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddSingleton<ISqlHelper, PostgresHelper>();
  }
}

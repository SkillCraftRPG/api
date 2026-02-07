using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Infrastructure;

namespace SkillCraft.Api;

internal class Program
{
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    IConfiguration configuration = builder.Configuration;

    Startup startup = new(configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    using IServiceScope scope = application.Services.CreateScope();
    using GameContext game = scope.ServiceProvider.GetRequiredService<GameContext>();
    await game.Database.MigrateAsync();

    application.Run();
  }
}

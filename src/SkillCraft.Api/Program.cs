using Logitar.EventSourcing.EntityFrameworkCore.Relational;
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
    using EventContext events = scope.ServiceProvider.GetRequiredService<EventContext>();
    using GameContext game = scope.ServiceProvider.GetRequiredService<GameContext>();
    await events.Database.MigrateAsync();
    await game.Database.MigrateAsync();

    application.Run();
  }
}

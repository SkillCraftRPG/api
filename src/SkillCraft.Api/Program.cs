using Logitar.CQRS;
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

    await MigrateDatabaseAsync(application);

    application.Run();
  }

  private static async Task MigrateDatabaseAsync(WebApplication application, CancellationToken cancellationToken = default)
  {
    using IServiceScope scope = application.Services.CreateScope();
    ICommandBus commandBus = scope.ServiceProvider.GetRequiredService<ICommandBus>();
    await commandBus.ExecuteAsync(new MigrateDatabaseCommand(), cancellationToken);
  }
}

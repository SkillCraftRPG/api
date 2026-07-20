using Bogus;
using Krakenar.Client.Users;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;
using Logitar;
using Logitar.CQRS;
using Logitar.Data;
using Logitar.Data.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Infrastructure;
using SkillCraft.Api.PostgreSQL;

namespace SkillCraft.Api.IntegrationTests;

public abstract class IntegrationTests : IAsyncLifetime
{
  private readonly Actor _system = new();

  protected virtual Faker Faker { get; set; }
  protected virtual TestContext Context { get; set; }

  protected virtual IConfiguration Configuration { get; set; }
  protected virtual IServiceProvider ServiceProvider { get; set; }

  protected virtual Actor Actor => Context.User is null ? _system : new(Context.User);
  protected virtual Mock<IUserClient> UserClient { get; set; } = new();

  protected IntegrationTests()
  {
    Faker = new();
    Context = new(Faker);

    Configuration = BuildConfiguration();
    ServiceProvider = BuildServiceProvider();

    Context.Database = ServiceProvider.GetRequiredService<GameContext>();
  }

  protected virtual IConfiguration BuildConfiguration() => new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

  protected virtual IServiceProvider BuildServiceProvider()
  {
    string? connectionString = EnvironmentHelper.TryGetString("POSTGRESQLCONNSTR_SkillCraft")
      ?? Configuration.GetConnectionString("PostgreSQL")
      ?? throw new InvalidOperationException("The PostgreSQL connection string was not found.");

    ServiceCollection services = new();
    services.AddSingleton(Configuration);

    services.AddSkillCraftApiCore();
    services.AddSkillCraftApiInfrastructure();
    services.AddSkillCraftApiPostgreSQL(connectionString.Replace("{Database}", GetType().Name));
    services.AddSingleton<IContext>(Context);
    services.AddSingleton(UserClient.Object);

    return services.BuildServiceProvider();
  }

  public virtual async Task InitializeAsync()
  {
    await MigrateDatabaseAsync();
    await ClearDatabaseAsync();
    await InitializeDatabaseAsync();
  }
  protected virtual async Task MigrateDatabaseAsync()
  {
    ICommandBus commandBus = ServiceProvider.GetRequiredService<ICommandBus>();
    await commandBus.ExecuteAsync(new MigrateDatabaseCommand());
  }
  protected virtual async Task ClearDatabaseAsync()
  {
    GameContext context = ServiceProvider.GetRequiredService<GameContext>();
    StringBuilder sql = new();
    TableId[] tables =
    [
      Infrastructure.Db.Castes.Table,
      Infrastructure.Db.Languages.Table,
      Infrastructure.Db.Scripts.Table,
      Infrastructure.Db.Customizations.Table,
      Infrastructure.Db.Worlds.Table,
      Infrastructure.Db.History.Table
    ];
    foreach (TableId table in tables)
    {
      sql.Append(new PostgresDeleteBuilder(table).Build().Text).Append(';').AppendLine();
    }
    await context.Database.ExecuteSqlRawAsync(sql.ToString());
  }
  protected virtual async Task InitializeDatabaseAsync()
  {
    Context.User = new UserBuilder(Faker).Build();
    UserClient.Setup(x => x.SearchAsync(
      It.Is<SearchUsersPayload>(p => p.Ids.Single() == Context.User.Id),
      It.IsAny<CancellationToken>())).ReturnsAsync(new SearchResults<User>([Context.User]));

    IWorldRepository worldRepository = ServiceProvider.GetRequiredService<IWorldRepository>();
    Context.World = new WorldBuilder(Faker).WithOwner(Context.User).Build();
    worldRepository.Add(Context.World);
    await Context.SaveChangesAsync();
  }

  public virtual Task DisposeAsync() => Task.CompletedTask;
}

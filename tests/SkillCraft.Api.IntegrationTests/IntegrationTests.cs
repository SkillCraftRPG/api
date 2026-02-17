using Bogus;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;
using Logitar;
using Logitar.Data;
using Logitar.Data.PostgreSQL;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SkillCraft.Api.Contracts.Worlds;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Infrastructure;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.PostgreSQL;
using GameDb = SkillCraft.Api.Infrastructure.GameDb;

namespace SkillCraft.Api;

public abstract class IntegrationTests : IAsyncLifetime
{
  protected Faker Faker { get; } = new();

  private readonly IntegrationTestContext _context = new();
  private readonly Mock<IUserService> _userService = new();

  protected Actor Actor => _context.User is null ? new() : new(_context.User);
  protected ActorId ActorId => ActorHelper.GetActorId(Actor);
  protected UserId UserId => new(ActorId);

  private World? _world = null;
  protected World World => _world ?? throw new InvalidOperationException("The world has not been initialized.");

  protected IServiceProvider ServiceProvider { get; }
  protected GameContext GameContext { get; }

  protected IntegrationTests()
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .AddUserSecrets("256e20b6-4956-4dcb-a7de-e5601517661b")
      .Build();

    string connectionString = (EnvironmentHelper.TryGetString("POSTGRESQLCONNSTR_Krakenar") ?? configuration.GetConnectionString("PostgreSQL"))
      ?.Replace("{Database}", GetType().Name)
      ?? throw new InvalidOperationException("The PostgreSQL connection string was not found.");

    ServiceCollection services = new();
    services.AddLogging();
    services.AddSingleton(configuration);

    services.AddSkillCraftApiCore();
    services.AddSkillCraftApiInfrastructure();
    services.AddSkillCraftApiPostgreSQL(connectionString);

    services.AddSingleton(_userService.Object);
    services.AddSingleton<IContext>(_context);

    ServiceProvider = services.BuildServiceProvider();
    GameContext = ServiceProvider.GetRequiredService<GameContext>();
  }

  public virtual async Task InitializeAsync()
  {
    EventContext eventContext = ServiceProvider.GetRequiredService<EventContext>();
    await eventContext.Database.MigrateAsync();
    await GameContext.Database.MigrateAsync();

    StringBuilder sql = new();
    TableId[] tables =
    [
      GameDb.StorageDetail.Table,
      GameDb.StorageSummary.Table,
      GameDb.Parties.Table,
      GameDb.Talents.Table,
      GameDb.Lineages.Table,
      GameDb.Languages.Table,
      GameDb.Scripts.Table,
      GameDb.Educations.Table,
      GameDb.Castes.Table,
      GameDb.Customizations.Table,
      GameDb.Worlds.Table,
      EventDb.Streams.Table
    ];
    foreach (TableId table in tables)
    {
      ICommand delete = new PostgresDeleteBuilder(table).Build();
      sql.Append(delete.Text).Append(';').AppendLine();
    }
    await GameContext.Database.ExecuteSqlRawAsync(sql.ToString());

    _context.User = new User
    {
      Id = Guid.NewGuid(),
      Realm = new Realm
      {
        Id = Guid.NewGuid(),
        UniqueSlug = "game",
        DisplayName = "Game"
      },
      UniqueName = Faker.Person.UserName,
      Email = new Email(Faker.Person.Email, isVerified: true),
      IsConfirmed = true,
      FirstName = Faker.Person.FirstName,
      LastName = Faker.Person.LastName,
      FullName = Faker.Person.FullName,
      Birthdate = Faker.Person.DateOfBirth,
      Gender = Faker.Person.Gender.ToString().ToLowerInvariant(),
      Locale = new Krakenar.Contracts.Localization.Locale(Faker.Locale),
      TimeZone = "America/Montreal",
      Picture = Faker.Person.Avatar,
      Website = Faker.Person.Website
    };
    _userService.Setup(x => x.SearchAsync(It.IsAny<SearchUsersPayload>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(new SearchResults<User>([_context.User]));

    _world = new World(UserId, new Name("Ungar"));
    _context.World = new WorldModel
    {
      Id = _world.Id.ToGuid(),
      Name = _world.Name.Value,
      Owner = Actor
    };
    IWorldRepository worldRepository = ServiceProvider.GetRequiredService<IWorldRepository>();
    await worldRepository.SaveAsync(_world);
  }

  public virtual Task DisposeAsync() => Task.CompletedTask;
}

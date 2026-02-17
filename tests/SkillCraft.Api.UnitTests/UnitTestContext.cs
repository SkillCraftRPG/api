using Bogus;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Users;
using Logitar.EventSourcing;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Infrastructure.Actors;

namespace SkillCraft.Api;

internal record UnitTestContext : IContext
{
  public UserId UserId { get; }

  public World World { get; }
  public WorldId WorldId => World.Id;

  public bool IsAdministrator { get; }
  public bool IsWorldOwner { get; }

  public UnitTestContext(World world)
  {
    UserId = world.OwnerId;

    World = world;

    IsWorldOwner = true;
  }

  public static UnitTestContext Generate(Faker? faker = null)
  {
    faker ??= new();

    User user = new()
    {
      Realm = new Realm
      {
        Id = Guid.NewGuid(),
        UniqueSlug = "ungar",
        DisplayName = "Ungar"
      },
      UniqueName = faker.Person.UserName,
      Email = new Email(faker.Person.Email, isVerified: true),
      IsConfirmed = true,
      FirstName = faker.Person.FirstName,
      LastName = faker.Person.LastName,
      FullName = faker.Person.FullName,
      Birthdate = faker.Person.DateOfBirth,
      Gender = faker.Person.Gender.ToString().ToLowerInvariant(),
      Locale = new Locale(faker.Locale),
      TimeZone = "America/Montreal",
      Picture = faker.Person.Avatar,
      Website = faker.Person.Website
    };
    Actor actor = new(user);
    ActorId actorId = ActorHelper.GetActorId(actor);
    UserId userId = new(actorId);
    World world = new(userId, new Name("Ungar"));

    return new UnitTestContext(world);
  }

  public UserId? TryGetUserId() => UserId;
  public WorldId? TryGetWorldId() => WorldId;
}

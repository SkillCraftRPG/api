using Bogus;
using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Users;
using Logitar.EventSourcing;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Actors;
using SkillCraft.Api.Core.Identity;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Infrastructure;

namespace SkillCraft.Api;

public class TestContext : IContext
{
  private readonly Faker _faker;

  public TestContext(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public GameContext? Database { get; set; }

  public ActorId? ActorId => User is null ? null : new Actor(User).GetActorId();

  public User? User { get; set; }
  public UserId UserId => TryGetUserId() ?? throw new InvalidOperationException("An authenticated user is required.");

  public World? World { get; set; }
  public WorldId WorldId => TryGetWorldId() ?? throw new InvalidOperationException("A world is required.");
  public Guid WorldUid => WorldId.ResourceId;
  public Guid UserUid => UserId.ResourceId;

  public IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes()
  {
    List<CustomAttribute> customAttributes = new(capacity: 2);
    customAttributes.Add(new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"));
    customAttributes.Add(new CustomAttribute("IpAddress", _faker.Internet.Ip()));
    return customAttributes.AsReadOnly();
  }

  public bool IsWorldOwner() => World is not null && User is not null && World.OwnerId == UserId;

  public Guid? TryGetSessionId() => null;
  public UserId? TryGetUserId() => User is null ? null : new UserId(new Actor(User).GetActorId());
  public WorldId? TryGetWorldId() => World?.Id;

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    if (Database is null)
    {
      throw new InvalidOperationException("The database was not initialized.");
    }

    return await Database.SaveChangesAsync(cancellationToken);
  }
}

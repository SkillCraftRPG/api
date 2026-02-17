using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Users;
using Logitar.EventSourcing;
using SkillCraft.Api.Contracts.Worlds;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Infrastructure.Actors;

namespace SkillCraft.Api;

internal class IntegrationTestContext : IContext
{
  public User? User { get; set; }
  public WorldModel? World { get; set; }

  public UserId UserId => TryGetUserId() ?? throw new InvalidOperationException("An authenticated user is required.");
  public WorldId WorldId => TryGetWorldId() ?? throw new InvalidOperationException("A world is required.");

  public bool IsAdministrator => User is not null && User.Roles.Any(role => role.UniqueName.Equals("admin", StringComparison.InvariantCultureIgnoreCase));
  public bool IsWorldOwner => User is not null && World is not null && World.Owner.RealmId == User.Realm?.Id && World.Owner.Id == User.Id;

  public UserId? TryGetUserId()
  {
    if (User is null)
    {
      return null;
    }
    Actor actor = new(User);
    ActorId actorId = ActorHelper.GetActorId(actor);
    return new UserId(actorId);
  }
  public WorldId? TryGetWorldId() => World is null ? null : new WorldId(World.Id);
}

using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Users;
using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Caching;

public interface ICacheService // TODO(fpion): remove user
{
  Actor? GetActor(ActorId id);
  void RemoveActor(ActorId id);
  void SetActor(Actor actor);

  User? GetUser(Guid id);
  void RemoveUser(Guid id);
  void SetUser(User user);
}

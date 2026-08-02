using Krakenar.Contracts;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Identity;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core;

public interface IContext
{
  ActorId? ActorId { get; }
  UserId UserId { get; }
  WorldId WorldId { get; }

  IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes();

  bool IsWorldOwner();

  Guid? TryGetSessionId();
  UserId? TryGetUserId();
  WorldId? TryGetWorldId();
}

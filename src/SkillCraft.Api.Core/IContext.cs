using Krakenar.Contracts;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Identity;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core;

public interface IContext
{
  ActorId? ActorId { get; }
  UserId UserId { get; }
  Guid UserUid { get; } // TODO(fpion): deprecate
  WorldId WorldId { get; }
  Guid WorldUid { get; } // TODO(fpion): deprecate

  IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes();

  bool IsWorldOwner();

  Guid? TryGetSessionId();
  UserId? TryGetUserId();
  Guid? TryGetUserUid(); // TODO(fpion): deprecate
  WorldId? TryGetWorldId();
  Guid? TryGetWorldUid(); // TODO(fpion): deprecate

  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); // TODO(fpion): deprecate
}

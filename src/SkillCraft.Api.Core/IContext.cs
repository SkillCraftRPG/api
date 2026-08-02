using Krakenar.Contracts;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core;

public interface IContext
{
  ActorId? ActorId { get; }
  Guid UserId { get; }
  WorldId WorldId { get; }
  Guid WorldUid { get; } // TODO(fpion): deprecate

  IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes();

  bool IsWorldOwner();

  Guid? TryGetSessionId();
  Guid? TryGetUserId();
  WorldId? TryGetWorldId();
  Guid? TryGetWorldUid(); // TODO(fpion): deprecate

  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); // TODO(fpion): deprecate
}

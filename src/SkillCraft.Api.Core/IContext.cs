using Krakenar.Contracts;

namespace SkillCraft.Api.Core;

public interface IContext
{
  Guid UserId { get; }
  Guid WorldId { get; }

  IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes();

  bool IsWorldOwner();

  Guid? TryGetUserId();
  Guid? TryGetWorldId();

  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

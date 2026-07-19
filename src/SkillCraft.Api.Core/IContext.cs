using Krakenar.Contracts;

namespace SkillCraft.Api.Core;

public interface IContext
{
  Guid UserId { get; }

  IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes();

  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

using Krakenar.Contracts.Realms;

namespace SkillCraft.Api.Core.Identity;

public interface IRealmGateway
{
  Task<Realm> FindAsync(CancellationToken cancellationToken = default);
}

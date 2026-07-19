using Krakenar.Contracts.Users;

namespace SkillCraft.Api.Core.Caching;

public interface ICacheService
{
  User? GetUser(Guid id);
  void RemoveUser(Guid id);
  void SetUser(User user);
}

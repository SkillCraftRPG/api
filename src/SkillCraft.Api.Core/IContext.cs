using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core;

public interface IContext
{
  UserId UserId { get; }
  WorldId WorldId { get; }

  bool IsAdministrator { get; }
  bool IsWorldOwner { get; }

  UserId? TryGetUserId();
  WorldId? TryGetWorldId();
}

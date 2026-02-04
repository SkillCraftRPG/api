using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core;

public interface IContext
{
  UserId UserId { get; }
  WorldId WorldId { get; }
}

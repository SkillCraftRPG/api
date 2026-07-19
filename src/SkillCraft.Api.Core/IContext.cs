using Krakenar.Contracts;

namespace SkillCraft.Api.Core;

public interface IContext
{
  IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes();
}

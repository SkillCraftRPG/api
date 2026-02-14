using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using SkillCraft.Api.Contracts.Worlds;

namespace SkillCraft.Api.Core.Logging;

public interface ILoggingService
{
  void SetApiKey(ApiKey? apiKey);
  void SetSession(Session? session);
  void SetUser(User? user);
  void SetWorld(WorldModel? world);
}

internal class LoggingService : ILoggingService
{
  public void SetApiKey(ApiKey? apiKey)
  {
  }

  public void SetSession(Session? session)
  {
  }

  public void SetUser(User? user)
  {
  }

  public void SetWorld(WorldModel? world)
  {
  }
}

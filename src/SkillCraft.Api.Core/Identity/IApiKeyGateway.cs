using Krakenar.Contracts.ApiKeys;

namespace SkillCraft.Api.Core.Identity;

public interface IApiKeyGateway
{
  Task<ApiKey> AuthenticateAsync(string xApiKey, CancellationToken cancellationToken = default);
}

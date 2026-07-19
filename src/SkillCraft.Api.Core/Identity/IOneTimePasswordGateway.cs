using SkillCraft.Api.Core.Identity.Models;
using Krakenar.Contracts.Passwords;
using Krakenar.Contracts.Users;

namespace SkillCraft.Api.Core.Identity;

public interface IOneTimePasswordGateway
{
  Task<OneTimePassword> CreateMultiFactorAuthenticationAsync(User user, CancellationToken cancellationToken = default);
  Task<User> ValidateMultiFactorAuthenticationAsync(OneTimePasswordValidation validation, CancellationToken cancellationToken = default);
}

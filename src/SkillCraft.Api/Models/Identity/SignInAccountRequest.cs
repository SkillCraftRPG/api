using SkillCraft.Api.Core.Identity.Models;

namespace SkillCraft.Api.Models.Identity;

public class SignInAccountRequest
{
  public Credentials? Credentials { get; set; }
  public string? AuthenticationToken { get; set; }
  public OneTimePasswordValidation? OneTimePassword { get; set; }
  public CompleteProfilePayload? Profile { get; set; }

  public SignInAccountPayload ToPayload() => new()
  {
    Credentials = Credentials,
    AuthenticationToken = AuthenticationToken,
    OneTimePassword = OneTimePassword,
    Profile = Profile
  };
}

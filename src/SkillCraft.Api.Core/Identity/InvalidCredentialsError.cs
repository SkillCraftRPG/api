using Krakenar.Contracts;

namespace SkillCraft.Api.Core.Identity;

public record InvalidCredentialsError : Error
{
  public InvalidCredentialsError() : base("InvalidCredentials", "The specified credentials did not match.")
  {
  }
}

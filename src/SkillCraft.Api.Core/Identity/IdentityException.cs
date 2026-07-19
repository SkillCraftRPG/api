namespace SkillCraft.Api.Core.Identity;

public abstract class IdentityException : Exception
{
  protected IdentityException(string? message, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}

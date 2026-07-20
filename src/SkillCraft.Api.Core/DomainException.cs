using Krakenar.Contracts;

namespace SkillCraft.Api.Core;

public abstract class DomainException : ErrorException
{
  protected DomainException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}

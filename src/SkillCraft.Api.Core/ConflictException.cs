using Krakenar.Contracts;

namespace SkillCraft.Api.Core;

public abstract class ConflictException : ErrorException
{
  protected ConflictException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}

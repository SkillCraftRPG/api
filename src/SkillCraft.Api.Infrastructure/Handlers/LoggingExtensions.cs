using Logitar.EventSourcing;
using Microsoft.Extensions.Logging;

namespace SkillCraft.Api.Infrastructure.Handlers;

internal static class LoggingExtensions
{
  public static void LogError<T>(this ILogger<T> logger, Exception exception, DomainEvent @event)
  {
    logger.LogError(exception, "An unhandled exception occurred while handling event '{Type} (Id={Id})'.", @event.GetType(), @event.Id);
  }
}

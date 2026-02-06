using Logitar;
using Microsoft.Extensions.Configuration;

namespace SkillCraft.Api.Infrastructure.Caching;

internal record CacheSettings
{
  private const string SectionKey = "Cache";

  public TimeSpan ActorLifetime { get; set; }

  public static CacheSettings Initialize(IConfiguration configuration)
  {
    CacheSettings settings = configuration.GetSection(SectionKey).Get<CacheSettings>() ?? new();

    settings.ActorLifetime = EnvironmentHelper.GetTimeSpan("CACHE_ACTOR_LIFETIME", settings.ActorLifetime);

    return settings;
  }
}

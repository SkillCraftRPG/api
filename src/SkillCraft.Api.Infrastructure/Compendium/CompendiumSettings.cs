using Logitar;
using Microsoft.Extensions.Configuration;

namespace SkillCraft.Api.Infrastructure.Compendium;

internal record CompendiumSettings
{
  private const string SectionKey = "Compendium";

  public string BaseUrl { get; set; } = string.Empty;
  public TimeSpan Timeout { get; set; }

  public static CompendiumSettings Initialize(IConfiguration configuration)
  {
    CompendiumSettings settings = configuration.GetSection(SectionKey).Get<CompendiumSettings>() ?? new();

    settings.BaseUrl = EnvironmentHelper.GetString("COMPENDIUM_BASE_URL", settings.BaseUrl);
    settings.Timeout = EnvironmentHelper.GetTimeSpan("COMPENDIUM_TIMEOUT", settings.Timeout);

    return settings;
  }
}

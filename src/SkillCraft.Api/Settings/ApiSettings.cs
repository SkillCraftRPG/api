using Logitar;

namespace SkillCraft.Api.Settings;

public record ApiSettings
{
  public const string SectionKey = "Api";

  public bool EnableBasicAuthentication { get; set; }
  public bool EnableSwagger { get; set; }
  public string Title { get; set; } = string.Empty;
  public Version Version { get; set; } = new();
  public string? Build { get; set; }

  public static ApiSettings Initialize(IConfiguration configuration)
  {
    ApiSettings settings = configuration.GetSection(SectionKey).Get<ApiSettings>() ?? new();

    settings.EnableBasicAuthentication = EnvironmentHelper.GetBoolean("API_ENABLE_BASIC_AUTHENTICATION", settings.EnableBasicAuthentication);
    settings.EnableSwagger = EnvironmentHelper.GetBoolean("API_ENABLE_SWAGGER", settings.EnableSwagger);
    settings.Title = EnvironmentHelper.GetString("API_TITLE", settings.Title);
    settings.Build = EnvironmentHelper.TryGetString("API_BUILD");

    string? versionValue = EnvironmentHelper.TryGetString("API_VERSION");
    if (Version.TryParse(versionValue, out Version? version))
    {
      settings.Version = version;
    }

    settings.Build = EnvironmentHelper.TryGetString("API_BUILD") ?? settings.Build;

    return settings;
  }
}

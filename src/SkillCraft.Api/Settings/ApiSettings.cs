using Logitar;

namespace SkillCraft.Api.Settings;

public record ApiSettings
{
  public const string SectionKey = "Api";

  public bool EnableSwagger { get; set; }
  public string Title { get; set; } = "SkillCraft API";
  public Version Version { get; set; } = new(0, 1, 0);

  public static ApiSettings Initialize(IConfiguration configuration)
  {
    ApiSettings settings = configuration.GetSection(SectionKey).Get<ApiSettings>() ?? new();

    settings.EnableSwagger = EnvironmentHelper.GetBoolean("ADMIN_ENABLE_SWAGGER", settings.EnableSwagger);
    settings.Title = EnvironmentHelper.GetString("ADMIN_TITLE", settings.Title);

    string? versionValue = EnvironmentHelper.TryGetString("ADMIN_VERSION");
    if (Version.TryParse(versionValue, out Version? version))
    {
      settings.Version = version;
    }

    return settings;
  }
}

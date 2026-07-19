using Logitar;

namespace SkillCraft.Api.Settings;

public record ErrorSettings
{
  private const string SectionKey = "Error";

  public bool ExposeDetail { get; set; }

  public static ErrorSettings Initialize(IConfiguration configuration)
  {
    ErrorSettings settings = configuration.GetSection(SectionKey).Get<ErrorSettings>() ?? new();

    settings.ExposeDetail = EnvironmentHelper.GetBoolean("ERROR_EXPOSE_DETAIL", settings.ExposeDetail);

    return settings;
  }
}

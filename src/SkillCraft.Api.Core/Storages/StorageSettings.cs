using Logitar;
using Microsoft.Extensions.Configuration;

namespace SkillCraft.Api.Core.Storages;

internal record StorageSettings
{
  private const string SectionKey = "Storage";

  public long AllocatedBytes { get; set; }

  public static StorageSettings Initialize(IConfiguration configuration)
  {
    StorageSettings settings = configuration.GetSection(SectionKey).Get<StorageSettings>() ?? new();

    string allocatedBytesValue = EnvironmentHelper.GetString("STORAGE_ALLOCATED_BYTES");
    if (long.TryParse(allocatedBytesValue, out long allocatedBytes))
    {
      settings.AllocatedBytes = allocatedBytes;
    }

    return settings;
  }
}

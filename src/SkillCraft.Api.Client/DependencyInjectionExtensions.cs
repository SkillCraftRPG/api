using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Client.Customizations;
using SkillCraft.Api.Client.Worlds;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Contracts.Worlds;

namespace SkillCraft.Api.Client;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApiClient(this IServiceCollection services)
  {
    return services
      .AddTransient<ICustomizationService, CustomizationClient>()
      .AddTransient<IWorldService, WorldClient>();
  }
}

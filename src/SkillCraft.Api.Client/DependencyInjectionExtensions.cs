using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Client.Customizations;
using SkillCraft.Api.Contracts.Customizations;

namespace SkillCraft.Api.Client;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApiClient(this IServiceCollection services)
  {
    return services.AddTransient<ICustomizationService, CustomizationClient>();
  }
}

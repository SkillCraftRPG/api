using Microsoft.Extensions.DependencyInjection;

namespace SkillCraft.Api.Client;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApiClient(this IServiceCollection services)
  {
    return services;
  }
}

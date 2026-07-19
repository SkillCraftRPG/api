using Logitar.CQRS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Identity;

namespace SkillCraft.Api.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApiCore(this IServiceCollection services)
  {
    IdentityService.Register(services);

    return services
      .AddSingleton(serviceProvider => RetrySettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddTransient<ICommandBus, CommandBus>()
      .AddTransient<IQueryBus, QueryBus>();
  }
}

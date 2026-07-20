using Logitar.CQRS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Identity;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApiCore(this IServiceCollection services)
  {
    CasteService.Register(services);
    CustomizationService.Register(services);
    IdentityService.Register(services);
    LanguageService.Register(services);
    PermissionService.Register(services);
    ScriptService.Register(services);
    WorldService.Register(services);

    return services
      .AddSingleton(serviceProvider => RetrySettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddTransient<ICommandBus, CommandBus>()
      .AddTransient<IQueryBus, QueryBus>();
  }
}

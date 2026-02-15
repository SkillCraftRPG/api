using Logitar.CQRS;
using Logitar.EventSourcing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Logging;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApiCore(this IServiceCollection services)
  {
    CasteService.Register(services);
    CustomizationService.Register(services);
    WorldService.Register(services);

    return services
      .AddLogitarEventSourcing()
      .AddSingleton(serviceProvider => PermissionSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton(serviceProvider => RetrySettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton(serviceProvider => StorageSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddTransient<ICommandBus, CommandBus>()
      .AddTransient<IQueryBus, QueryBus>()
      .AddTransient<ILoggingService, LoggingService>()
      .AddTransient<IPermissionService, PermissionService>()
      .AddTransient<IStorageService, StorageService>();
  }
}

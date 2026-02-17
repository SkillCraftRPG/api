using Logitar.CQRS;
using Logitar.EventSourcing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Logging;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Parties;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Specializations;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApiCore(this IServiceCollection services)
  {
    CasteService.Register(services);
    EducationService.Register(services);
    CustomizationService.Register(services);
    LanguageService.Register(services);
    LineageService.Register(services);
    PartyService.Register(services);
    ScriptService.Register(services);
    SpecializationService.Register(services);
    TalentService.Register(services);
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

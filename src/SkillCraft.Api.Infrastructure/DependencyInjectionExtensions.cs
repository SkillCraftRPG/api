using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Parties;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Caching;
using SkillCraft.Api.Infrastructure.Handlers;
using SkillCraft.Api.Infrastructure.Queriers;
using SkillCraft.Api.Infrastructure.Repositories;

namespace SkillCraft.Api.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApiInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddEventHandlers()
      .AddMemoryCache()
      .AddQueriers()
      .AddRepositories()
      .AddSingleton(serviceProvider => CacheSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer, EventSerializer>()
      .AddScoped<IEventBus, EventBus>()
      .AddTransient<IActorService, ActorService>();
  }

  private static IServiceCollection AddEventHandlers(this IServiceCollection services)
  {
    CasteEvents.Register(services);
    CustomizationEvents.Register(services);
    EducationEvents.Register(services);
    PartyEvents.Register(services);
    ScriptEvents.Register(services);
    StorageEvents.Register(services);
    WorldEvents.Register(services);
    return services;
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddTransient<ICasteQuerier, CasteQuerier>()
      .AddTransient<ICustomizationQuerier, CustomizationQuerier>()
      .AddTransient<IEducationQuerier, EducationQuerier>()
      .AddTransient<IPartyQuerier, PartyQuerier>()
      .AddTransient<IScriptQuerier, ScriptQuerier>()
      .AddTransient<IStorageQuerier, StorageQuerier>()
      .AddTransient<IWorldQuerier, WorldQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddTransient<ICasteRepository, CasteRepository>()
      .AddTransient<ICustomizationRepository, CustomizationRepository>()
      .AddTransient<IEducationRepository, EducationRepository>()
      .AddTransient<IPartyRepository, PartyRepository>()
      .AddTransient<IScriptRepository, ScriptRepository>()
      .AddTransient<IStorageRepository, StorageRepository>()
      .AddTransient<IWorldRepository, WorldRepository>();
  }
}

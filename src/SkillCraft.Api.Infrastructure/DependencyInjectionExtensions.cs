using Logitar.CQRS;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Identity;
using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Spells;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Caching;
using SkillCraft.Api.Infrastructure.Identity;
using SkillCraft.Api.Infrastructure.Queriers;
using SkillCraft.Api.Infrastructure.Repositories;

namespace SkillCraft.Api.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApiInfrastructure(this IServiceCollection services)
  {
    ActorService.Register(services);
    CacheService.Register(services);

    return services
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddSingleton(serviceProvider => ClientAppSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton(serviceProvider => TokensSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton<IEventSerializer, EventSerializer>()
      .AddScoped<IEventBus, EventBus>()
      .AddIdentityGateways()
      .AddQueriers()
      .AddRepositories()
      .AddTransient<ICommandHandler<MigrateDatabaseCommand, Unit>, MigrateDatabaseCommandHandler>();
  }

  private static IServiceCollection AddIdentityGateways(this IServiceCollection services)
  {
    return services
      .AddSingleton<IApiKeyGateway, ApiKeyGateway>()
      .AddSingleton<IMessageGateway, MessageGateway>()
      .AddSingleton<IOneTimePasswordGateway, OneTimePasswordGateway>()
      .AddSingleton<IRealmGateway, RealmGateway>()
      .AddSingleton<ISessionGateway, SessionGateway>()
      .AddSingleton<ITokenGateway, TokenGateway>()
      .AddSingleton<IUserGateway, UserGateway>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<ITalentQuerier, TalentQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<ICasteRepository, CasteRepository>()
      .AddScoped<ICustomizationRepository, CustomizationRepository>()
      .AddScoped<IEducationRepository, EducationRepository>()
      .AddScoped<IItemRepository, ItemRepository>()
      .AddScoped<ILanguageRepository, LanguageRepository>()
      .AddScoped<ILineageRepository, LineageRepository>()
      .AddScoped<IScriptRepository, ScriptRepository>()
      .AddScoped<ISpellRepository, SpellRepository>()
      .AddScoped<ITalentRepository, TalentRepository>()
      .AddScoped<IWorldRepository, WorldRepository>();
  }
}

using Logitar.CQRS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Identity;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Caching;
using SkillCraft.Api.Infrastructure.Identity;

namespace SkillCraft.Api.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApiInfrastructure(this IServiceCollection services)
  {
    ActorService.Register(services);
    CacheService.Register(services);

    return services
      .AddSingleton(serviceProvider => ClientAppSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton(serviceProvider => TokensSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddIdentityGateways()
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
}

using Logitar.CQRS;
using Logitar.EventSourcing;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Customizations;

namespace SkillCraft.Api.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApiCore(this IServiceCollection services)
  {
    CustomizationService.Register(services);

    return services
      .AddLogitarCQRS()
      .AddLogitarEventSourcing();
  }
}

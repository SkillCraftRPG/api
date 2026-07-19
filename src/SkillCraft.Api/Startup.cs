using Krakenar.Client;
using Krakenar.Contracts.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using SkillCraft.Api.Authentication;
using SkillCraft.Api.Core;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Infrastructure;
using SkillCraft.Api.Middlewares;
using SkillCraft.Api.PostgreSQL;
using SkillCraft.Api.Settings;

namespace SkillCraft.Api;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;
  private readonly ApiSettings _apiSettings;
  private readonly CorsSettings _corsSettings;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
    _apiSettings = ApiSettings.Initialize(configuration);
    _corsSettings = CorsSettings.Initialize(configuration);
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddSkillCraftApiCore();
    services.AddSkillCraftApiInfrastructure();
    services.AddSkillCraftApiPostgreSQL(_configuration);
    services.AddSingleton<IContext, HttpApplicationContext>();

    services.AddKrakenarClient(_configuration);

    services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    services.AddHttpContextAccessor();

    services.AddSingleton(_corsSettings);
    services.AddCors();

    string[] authenticationSchemes = GetAuthenticationSchemes();
    AuthenticationBuilder authenticationBuilder = services
      .AddAuthentication()
      .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Schemes.ApiKey, options => { })
      .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(Schemes.Bearer, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });
    if (_apiSettings.EnableBasicAuthentication)
    {
      authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });
    }

    services.AddAuthorizationBuilder().SetDefaultPolicy(new AuthorizationPolicyBuilder(authenticationSchemes).RequireAuthenticatedUser().Build());

    CookiesSettings cookiesSettings = CookiesSettings.Initialize(_configuration);
    services.AddSingleton(cookiesSettings);
    services.AddSession(options =>
    {
      options.Cookie.SameSite = cookiesSettings.Session.SameSite;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
    services.AddDistributedMemoryCache();

    ErrorSettings errorSettings = ErrorSettings.Initialize(_configuration);
    services.AddSingleton(errorSettings);
    services.AddExceptionHandler<ExceptionHandler>();
    services.AddProblemDetails();

    services.AddHealthChecks().AddDbContextCheck<GameContext>();

    services.AddSingleton(_apiSettings);
    if (_apiSettings.EnableSwagger)
    {
      services.AddSkillCraftSwagger(_apiSettings);
    }
  }
  private string[] GetAuthenticationSchemes()
  {
    List<string> schemes = new(capacity: 4)
    {
      Schemes.ApiKey,
      Schemes.Bearer,
      Schemes.Session
    };

    if (_apiSettings.EnableBasicAuthentication)
    {
      schemes.Add(Schemes.Basic);
    }

    return [.. schemes];
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (builder is WebApplication application)
    {
      Configure(application);
    }
  }
  public virtual void Configure(WebApplication application)
  {
    ApiSettings apiSettings = application.Services.GetRequiredService<ApiSettings>();
    if (apiSettings.EnableSwagger)
    {
      application.UseSkillCraftSwagger(apiSettings);
    }

    application.UseHttpsRedirection();
    application.UseCors(_corsSettings);
    application.UseExceptionHandler();
    application.UseSession();
    application.UseMiddleware<RenewSession>();
    application.UseAuthentication();
    application.UseAuthorization();
    application.UseMiddleware<ResolveWorld>();
    application.MapControllers();

    application.MapControllers();
    application.MapHealthChecks("/health");
  }
}

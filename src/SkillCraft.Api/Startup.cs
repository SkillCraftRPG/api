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

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddSkillCraftApiCore();
    services.AddSkillCraftApiInfrastructure();
    services.AddSkillCraftApiPostgreSQL(_configuration);

    services.AddHttpContextAccessor();
    services.AddSingleton<IContext, HttpApplicationContext>();

    services.AddControllersWithViews().AddJsonOptions(options =>
    {
      options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
      options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

    services.AddSingleton(CorsSettings.Initialize(_configuration));
    services.AddCors();

    AuthenticationSettings authenticationSettings = AuthenticationSettings.Initialize(_configuration);
    services.AddSingleton(authenticationSettings);
    string[] authenticationSchemes = authenticationSettings.GetAuthenticationSchemes();
    AuthenticationBuilder authenticationBuilder = services
      .AddAuthentication()
      .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Schemes.ApiKey, options => { })
      .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(Schemes.Bearer, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });
    if (authenticationSettings.EnableBasic)
    {
      authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });
    }
    services.AddTransient<IOpenAuthenticationService, OpenAuthenticationService>();

    services.AddAuthorizationBuilder()
      .SetDefaultPolicy(new AuthorizationPolicyBuilder(authenticationSchemes).RequireAuthenticatedUser().Build());

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

    ApiSettings apiSettings = ApiSettings.Initialize(_configuration);
    services.AddSingleton(apiSettings);
    if (apiSettings.EnableSwagger)
    {
      services.AddSkillCraftSwagger(apiSettings);
    }

    services.AddApplicationInsightsTelemetry();
    services.AddHealthChecks().AddDbContextCheck<GameContext>();
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
    application.UseCors();
    application.UseExceptionHandler();
    application.UseSession();
    application.UseMiddleware<RenewSession>();
    application.UseAuthentication();
    application.UseAuthorization();
    application.MapControllers();
    application.UseMiddleware<ResolveWorld>();

    application.MapControllers();
    application.MapHealthChecks("/health");
  }
}

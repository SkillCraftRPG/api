using Krakenar.Contracts.Search;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Languages.Models;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace SkillCraft.Api.Infrastructure.Compendium;

public interface ICompendiumService
{
  Task<SearchResults<LanguageModel>> GetLanguagesAsync(CancellationToken cancellationToken = default);
}

internal class CompendiumService : ICompendiumService
{
  public static void Register(IServiceCollection services)
  {
    services.AddSingleton(serviceProvider => CompendiumSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()));
    services.AddSingleton<ICompendiumService, CompendiumService>();
  }

  private readonly HttpClient _client = new();
  private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

  public CompendiumService(CompendiumSettings settings)
  {
    _client.BaseAddress = new Uri(settings.BaseUrl, UriKind.Absolute);
    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
    _client.Timeout = settings.Timeout;

    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public async Task<SearchResults<LanguageModel>> GetLanguagesAsync(CancellationToken cancellationToken)
  {
    using HttpRequestMessage request = new(HttpMethod.Get, new Uri("/api/rules/languages?sort=Name", UriKind.Relative));
    using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
    response.EnsureSuccessStatusCode();

    string json = Format(await response.Content.ReadAsStringAsync(cancellationToken));
    return JsonSerializer.Deserialize<SearchResults<LanguageModel>>(json, _serializerOptions) ?? new();
  }

  private static string Format(string json) => json.Replace(@"""htmlContent"":", @"""content"":");
}

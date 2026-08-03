using Krakenar.Contracts.Search;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Core.Educations.Models;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Core.Scripts.Models;
using SkillCraft.Api.Core.Talents.Models;
using SkillCraft.Api.Infrastructure.Compendium.Models;

namespace SkillCraft.Api.Infrastructure.Compendium;

public interface ICompendiumService
{
  Task<SearchResults<CasteModel>> GetCastesAsync(CancellationToken cancellationToken = default);
  Task<SearchResults<CustomizationModel>> GetCustomizationsAsync(CancellationToken cancellationToken = default);
  Task<SearchResults<EducationModel>> GetEducationsAsync(CancellationToken cancellationToken = default);
  Task<SearchResults<LanguageModel>> GetLanguagesAsync(CancellationToken cancellationToken = default);
  Task<SearchResults<ScriptModel>> GetScriptsAsync(CancellationToken cancellationToken = default);
  Task<SearchResults<TalentModel>> GetTalentsAsync(CancellationToken cancellationToken = default);
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

  public async Task<SearchResults<CasteModel>> GetCastesAsync(CancellationToken cancellationToken)
  {
    using HttpRequestMessage request = new(HttpMethod.Get, new Uri("/api/castes", UriKind.Relative));
    using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
    response.EnsureSuccessStatusCode();

    SearchResults<CasteEntry> entries = await response.Content.ReadFromJsonAsync<SearchResults<CasteEntry>>(_serializerOptions, cancellationToken) ?? new();
    return new SearchResults<CasteModel>(entries.Items.Select(CompendiumMapper.ToCaste), entries.Total);
  }

  public async Task<SearchResults<CustomizationModel>> GetCustomizationsAsync(CancellationToken cancellationToken)
  {
    using HttpRequestMessage request = new(HttpMethod.Get, new Uri("/api/customizations", UriKind.Relative));
    using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
    response.EnsureSuccessStatusCode();

    SearchResults<CustomizationEntry> entries = await response.Content.ReadFromJsonAsync<SearchResults<CustomizationEntry>>(_serializerOptions, cancellationToken) ?? new();
    return new SearchResults<CustomizationModel>(entries.Items.Select(CompendiumMapper.ToCustomization), entries.Total);
  }

  public async Task<SearchResults<EducationModel>> GetEducationsAsync(CancellationToken cancellationToken)
  {
    using HttpRequestMessage request = new(HttpMethod.Get, new Uri("/api/educations", UriKind.Relative));
    using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
    response.EnsureSuccessStatusCode();

    SearchResults<EducationEntry> entries = await response.Content.ReadFromJsonAsync<SearchResults<EducationEntry>>(_serializerOptions, cancellationToken) ?? new();
    return new SearchResults<EducationModel>(entries.Items.Select(CompendiumMapper.ToEducation), entries.Total);
  }

  public async Task<SearchResults<LanguageModel>> GetLanguagesAsync(CancellationToken cancellationToken)
  {
    using HttpRequestMessage request = new(HttpMethod.Get, new Uri("/api/rules/languages", UriKind.Relative));
    using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
    response.EnsureSuccessStatusCode();

    SearchResults<LanguageEntry> entries = await response.Content.ReadFromJsonAsync<SearchResults<LanguageEntry>>(_serializerOptions, cancellationToken) ?? new();
    return new SearchResults<LanguageModel>(entries.Items.Select(CompendiumMapper.ToLanguage), entries.Total);
  }

  public async Task<SearchResults<ScriptModel>> GetScriptsAsync(CancellationToken cancellationToken)
  {
    using HttpRequestMessage request = new(HttpMethod.Get, new Uri("/api/scripts", UriKind.Relative));
    using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
    response.EnsureSuccessStatusCode();

    SearchResults<ScriptEntry> entries = await response.Content.ReadFromJsonAsync<SearchResults<ScriptEntry>>(_serializerOptions, cancellationToken) ?? new();
    return new SearchResults<ScriptModel>(entries.Items.Select(CompendiumMapper.ToScript), entries.Total);
  }

  public async Task<SearchResults<TalentModel>> GetTalentsAsync(CancellationToken cancellationToken)
  {
    using HttpRequestMessage request = new(HttpMethod.Get, new Uri("/api/talents", UriKind.Relative));
    using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
    response.EnsureSuccessStatusCode();

    SearchResults<TalentEntry> entries = await response.Content.ReadFromJsonAsync<SearchResults<TalentEntry>>(_serializerOptions, cancellationToken) ?? new();
    return new SearchResults<TalentModel>(entries.Items.Select(CompendiumMapper.ToTalent), entries.Total);
  }
}

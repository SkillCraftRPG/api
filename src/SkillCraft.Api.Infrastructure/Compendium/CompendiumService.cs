using Krakenar.Contracts.Search;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Core.Scripts.Models;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json.Nodes;

namespace SkillCraft.Api.Infrastructure.Compendium;

public interface ICompendiumService
{
  Task<SearchResults<CasteModel>> GetCastesAsync(CancellationToken cancellationToken = default);
  Task<SearchResults<CustomizationModel>> GetCustomizationsAsync(CancellationToken cancellationToken = default);
  Task<SearchResults<LanguageModel>> GetLanguagesAsync(CancellationToken cancellationToken = default);
  Task<SearchResults<ScriptModel>> GetScriptsAsync(CancellationToken cancellationToken = default);
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

    string json = Format(await response.Content.ReadAsStringAsync(cancellationToken));
    return JsonSerializer.Deserialize<SearchResults<CasteModel>>(json, _serializerOptions) ?? new();
  }

  public async Task<SearchResults<CustomizationModel>> GetCustomizationsAsync(CancellationToken cancellationToken)
  {
    using HttpRequestMessage request = new(HttpMethod.Get, new Uri("/api/customizations", UriKind.Relative));
    using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
    response.EnsureSuccessStatusCode();

    string json = Format(await response.Content.ReadAsStringAsync(cancellationToken));
    return JsonSerializer.Deserialize<SearchResults<CustomizationModel>>(json, _serializerOptions) ?? new();
  }

  public async Task<SearchResults<LanguageModel>> GetLanguagesAsync(CancellationToken cancellationToken)
  {
    using HttpRequestMessage request = new(HttpMethod.Get, new Uri("/api/rules/languages", UriKind.Relative));
    using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
    response.EnsureSuccessStatusCode();

    string json = Format(await response.Content.ReadAsStringAsync(cancellationToken));
    return JsonSerializer.Deserialize<SearchResults<LanguageModel>>(json, _serializerOptions) ?? new();
  }

  public async Task<SearchResults<ScriptModel>> GetScriptsAsync(CancellationToken cancellationToken)
  {
    using HttpRequestMessage request = new(HttpMethod.Get, new Uri("/api/scripts", UriKind.Relative));
    using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
    response.EnsureSuccessStatusCode();

    string json = Format(await response.Content.ReadAsStringAsync(cancellationToken));
    return JsonSerializer.Deserialize<SearchResults<ScriptModel>>(json, _serializerOptions) ?? new();
  }

  private static string Format(string json)
  {
    JsonNode? root = JsonNode.Parse(json.Replace(@"""htmlContent"":", @"""content"":"));
    if (root is null)
    {
      return json;
    }

    FormatSkills(root);
    return root.ToJsonString();
  }

  private static void FormatSkills(JsonNode node)
  {
    if (node is JsonObject obj)
    {
      if (obj["skill"] is JsonObject skill && skill["value"] is JsonValue value)
      {
        obj["skill"] = value.DeepClone();
      }

      foreach (KeyValuePair<string, JsonNode?> property in obj.ToList())
      {
        if (property.Value is not null)
        {
          FormatSkills(property.Value);
        }
      }
    }
    else if (node is JsonArray array)
    {
      foreach (JsonNode? item in array)
      {
        if (item is not null)
        {
          FormatSkills(item);
        }
      }
    }
  }
}

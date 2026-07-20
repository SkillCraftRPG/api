using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Filters;
using SkillCraft.Api.Models.Language;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("languages")]
public class LanguageController : ControllerBase
{
  private readonly ILanguageService _languageService;

  public LanguageController(ILanguageService languageService)
  {
    _languageService = languageService;
  }

  [HttpPost]
  public async Task<ActionResult<LanguageModel>> CreateAsync([FromBody] CreateOrReplaceLanguagePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguageResult result = await _languageService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<LanguageModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _languageService.ReadAsync(id, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<LanguageModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceLanguagePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguageResult result = await _languageService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<LanguageModel>>> SearchAsync([FromQuery] SearchLanguagesParameters parameters, CancellationToken cancellationToken)
  {
    SearchLanguagesPayload payload = parameters.ToPayload();
    SearchResults<LanguageModel> languages = await _languageService.SearchAsync(payload, cancellationToken);
    return Ok(languages);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<LanguageModel>> UpdateAsync(Guid id, [FromBody] UpdateLanguagePayload payload, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _languageService.UpdateAsync(id, payload, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  private ActionResult<LanguageModel> ToActionResult(CreateOrReplaceLanguageResult result)
  {
    LanguageModel language = result.Language;
    if (result.Created)
    {
      Uri location = new($"{HttpContext.GetBaseUrl()}/languages/{language.Id}", UriKind.Absolute);
      return Created(location, language);
    }
    return Ok(language);
  }
}

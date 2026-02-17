using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Languages;
using SkillCraft.Api.Models.Parameters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[Route("languages")]
public class LanguageController : ControllerBase
{
  private readonly ILanguageService _languageService;

  public LanguageController(ILanguageService languageService)
  {
    _languageService = languageService;
  }

  [HttpPost]
  [ProducesResponseType<LanguageModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<LanguageModel>> CreateAsync([FromBody] CreateOrReplaceLanguagePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguageResult result = await _languageService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  [ProducesResponseType<LanguageModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<LanguageModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _languageService.DeleteAsync(id, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  [HttpGet("{id}")]
  [ProducesResponseType<LanguageModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<LanguageModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _languageService.ReadAsync(id, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  [HttpPut("{id}")]
  [ProducesResponseType<LanguageModel>(StatusCodes.Status200OK)]
  [ProducesResponseType<LanguageModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<LanguageModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceLanguagePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguageResult result = await _languageService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  [ProducesResponseType<LanguageModel>(StatusCodes.Status200OK)]
  public async Task<ActionResult<SearchResults<LanguageModel>>> SearchAsync(SearchLanguagesParameters parameters, CancellationToken cancellationToken)
  {
    SearchLanguagesPayload payload = parameters.ToPayload();
    SearchResults<LanguageModel> languages = await _languageService.SearchAsync(payload, cancellationToken);
    return Ok(languages);
  }

  [HttpPatch("{id}")]
  [ProducesResponseType<LanguageModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
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
      Uri location = new($"{Request.Scheme}://{Request.Host}/languages/{language.Id}", UriKind.Absolute);
      return Created(location, language);
    }
    return Ok(language);
  }
}

using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Scripts;
using SkillCraft.Api.Models.Parameters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[Route("scripts")]
public class ScriptController : ControllerBase
{
  private readonly IScriptService _scriptService;

  public ScriptController(IScriptService scriptService)
  {
    _scriptService = scriptService;
  }

  [HttpPost]
  [ProducesResponseType<ScriptModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<ScriptModel>> CreateAsync([FromBody] CreateOrReplaceScriptPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceScriptResult result = await _scriptService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  [ProducesResponseType<ScriptModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ScriptModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    ScriptModel? script = await _scriptService.DeleteAsync(id, cancellationToken);
    return script is null ? NotFound() : Ok(script);
  }

  [HttpGet("{id}")]
  [ProducesResponseType<ScriptModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ScriptModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ScriptModel? script = await _scriptService.ReadAsync(id, cancellationToken);
    return script is null ? NotFound() : Ok(script);
  }

  [HttpPut("{id}")]
  [ProducesResponseType<ScriptModel>(StatusCodes.Status200OK)]
  [ProducesResponseType<ScriptModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<ScriptModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceScriptPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceScriptResult result = await _scriptService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  [ProducesResponseType<ScriptModel>(StatusCodes.Status200OK)]
  public async Task<ActionResult<SearchResults<ScriptModel>>> SearchAsync(SearchScriptsParameters parameters, CancellationToken cancellationToken)
  {
    SearchScriptsPayload payload = parameters.ToPayload();
    SearchResults<ScriptModel> scripts = await _scriptService.SearchAsync(payload, cancellationToken);
    return Ok(scripts);
  }

  [HttpPatch("{id}")]
  [ProducesResponseType<ScriptModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ScriptModel>> UpdateAsync(Guid id, [FromBody] UpdateScriptPayload payload, CancellationToken cancellationToken)
  {
    ScriptModel? script = await _scriptService.UpdateAsync(id, payload, cancellationToken);
    return script is null ? NotFound() : Ok(script);
  }

  private ActionResult<ScriptModel> ToActionResult(CreateOrReplaceScriptResult result)
  {
    ScriptModel script = result.Script;
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/scripts/{script.Id}", UriKind.Absolute);
      return Created(location, script);
    }
    return Ok(script);
  }
}

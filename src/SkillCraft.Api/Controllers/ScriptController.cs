using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Scripts.Models;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Filters;
using SkillCraft.Api.Models.Script;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("scripts")]
public class ScriptController : ControllerBase
{
  private readonly IScriptService _scriptService;

  public ScriptController(IScriptService scriptService)
  {
    _scriptService = scriptService;
  }

  [HttpPost]
  public async Task<ActionResult<ScriptModel>> CreateAsync([FromBody] CreateOrReplaceScriptPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceScriptResult result = await _scriptService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ScriptModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ScriptModel? script = await _scriptService.ReadAsync(id, cancellationToken);
    return script is null ? NotFound() : Ok(script);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ScriptModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceScriptPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceScriptResult result = await _scriptService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ScriptModel>>> SearchAsync([FromQuery] SearchScriptsParameters parameters, CancellationToken cancellationToken)
  {
    SearchScriptsPayload payload = parameters.ToPayload();
    SearchResults<ScriptModel> scripts = await _scriptService.SearchAsync(payload, cancellationToken);
    return Ok(scripts);
  }

  [HttpPatch("{id}")]
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
      Uri location = new($"{HttpContext.GetBaseUrl()}/scripts/{script.Id}", UriKind.Absolute);
      return Created(location, script);
    }
    return Ok(script);
  }
}

using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Filters;
using SkillCraft.Api.Models.Lineage;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("lineages")]
public class LineageController : ControllerBase
{
  private readonly ILineageService _lineageService;

  public LineageController(ILineageService lineageService)
  {
    _lineageService = lineageService;
  }

  [HttpPost]
  public async Task<ActionResult<LineageModel>> CreateAsync([FromBody] CreateOrReplaceLineagePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLineageResult result = await _lineageService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<LineageModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LineageModel? lineage = await _lineageService.ReadAsync(id, cancellationToken);
    return lineage is null ? NotFound() : Ok(lineage);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<LineageModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceLineagePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLineageResult result = await _lineageService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<LineageModel>>> SearchAsync([FromQuery] SearchLineagesParameters parameters, CancellationToken cancellationToken)
  {
    SearchLineagesPayload payload = parameters.ToPayload();
    SearchResults<LineageModel> lineages = await _lineageService.SearchAsync(payload, cancellationToken);
    return Ok(lineages);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<LineageModel>> UpdateAsync(Guid id, [FromBody] UpdateLineagePayload payload, CancellationToken cancellationToken)
  {
    LineageModel? lineage = await _lineageService.UpdateAsync(id, payload, cancellationToken);
    return lineage is null ? NotFound() : Ok(lineage);
  }

  private ActionResult<LineageModel> ToActionResult(CreateOrReplaceLineageResult result)
  {
    LineageModel lineage = result.Lineage;
    if (result.Created)
    {
      Uri location = new($"{HttpContext.GetBaseUrl()}/lineages/{lineage.Id}", UriKind.Absolute);
      return Created(location, lineage);
    }
    return Ok(lineage);
  }
}

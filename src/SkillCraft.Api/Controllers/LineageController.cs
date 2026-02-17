using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Models.Parameters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[Route("lineages")]
public class LineageController : ControllerBase
{
  private readonly ILineageService _lineageService;

  public LineageController(ILineageService lineageService)
  {
    _lineageService = lineageService;
  }

  [HttpPost]
  [ProducesResponseType<LineageModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<LineageModel>> CreateAsync([FromBody] CreateOrReplaceLineagePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLineageResult result = await _lineageService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  [ProducesResponseType<LineageModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<LineageModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    LineageModel? lineage = await _lineageService.DeleteAsync(id, cancellationToken);
    return lineage is null ? NotFound() : Ok(lineage);
  }

  [HttpGet("{id}")]
  [ProducesResponseType<LineageModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<LineageModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LineageModel? lineage = await _lineageService.ReadAsync(id, cancellationToken);
    return lineage is null ? NotFound() : Ok(lineage);
  }

  [HttpPut("{id}")]
  [ProducesResponseType<LineageModel>(StatusCodes.Status200OK)]
  [ProducesResponseType<LineageModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<LineageModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceLineagePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLineageResult result = await _lineageService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  [ProducesResponseType<LineageModel>(StatusCodes.Status200OK)]
  public async Task<ActionResult<SearchResults<LineageModel>>> SearchAsync(SearchLineagesParameters parameters, CancellationToken cancellationToken)
  {
    SearchLineagesPayload payload = parameters.ToPayload();
    SearchResults<LineageModel> lineages = await _lineageService.SearchAsync(payload, cancellationToken);
    return Ok(lineages);
  }

  [HttpPatch("{id}")]
  [ProducesResponseType<LineageModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
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
      Uri location = new($"{Request.Scheme}://{Request.Host}/lineages/{lineage.Id}", UriKind.Absolute);
      return Created(location, lineage);
    }
    return Ok(lineage);
  }
}

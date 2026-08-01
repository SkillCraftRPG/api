using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Filters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("lineages/{lineageId}/features")]
public class LineageFeatureController : ControllerBase
{
  private readonly ILineageService _lineageService;

  public LineageFeatureController(ILineageService lineageService)
  {
    _lineageService = lineageService;
  }

  [HttpPost]
  public async Task<ActionResult<LineageModel>> CreateAsync(Guid lineageId, [FromBody] FeatureModel payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLineageFeatureResult result = await _lineageService.CreateOrReplaceFeatureAsync(lineageId, payload, featureId: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{featureId}")]
  public async Task<ActionResult<LineageModel>> DeleteAsync(Guid lineageId, Guid featureId, CancellationToken cancellationToken)
  {
    LineageModel? lineage = await _lineageService.DeleteFeatureAsync(lineageId, featureId, cancellationToken);
    return lineage is null ? NotFound() : Ok(lineage);
  }

  [HttpGet("{featureId}")]
  public async Task<ActionResult<LineageModel>> ReadAsync(Guid lineageId, Guid featureId, CancellationToken cancellationToken)
  {
    LineageModel? lineage = await _lineageService.ReadAsync(lineageId, cancellationToken);
    LineageFeatureModel? feature = lineage?.Features.SingleOrDefault(feature => feature.Id == featureId);
    return feature is null ? NotFound() : Ok(feature);
  }

  [HttpPut("{featureId}")]
  public async Task<ActionResult<LineageModel>> ReplaceAsync(Guid lineageId, Guid featureId, [FromBody] FeatureModel payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLineageFeatureResult result = await _lineageService.CreateOrReplaceFeatureAsync(lineageId, payload, featureId, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<LineageModel>>> SearchAsync(Guid lineageId, CancellationToken cancellationToken)
  {
    LineageModel? lineage = await _lineageService.ReadAsync(lineageId, cancellationToken);
    if (lineage is null)
    {
      return NotFound();
    }

    SearchResults<LineageFeatureModel> results = new(lineage.Features);
    return Ok(results);
  }

  private ActionResult<LineageModel> ToActionResult(CreateOrReplaceLineageFeatureResult result)
  {
    LineageModel lineage = result.Lineage;
    if (result.Created)
    {
      Uri location = new($"{HttpContext.GetBaseUrl()}/lineages/{lineage.Id}/features/{result.FeatureId}", UriKind.Absolute);
      return Created(location, lineage);
    }
    return Ok(lineage);
  }
}

using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Filters;
using SkillCraft.Api.Models.Caste;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("castes")]
public class CasteController : ControllerBase
{
  private readonly ICasteService _casteService;

  public CasteController(ICasteService casteService)
  {
    _casteService = casteService;
  }

  [HttpPost]
  public async Task<ActionResult<CasteModel>> CreateAsync([FromBody] CreateOrReplaceCastePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceCasteResult result = await _casteService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<CasteModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CasteModel? caste = await _casteService.ReadAsync(id, cancellationToken);
    return caste is null ? NotFound() : Ok(caste);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<CasteModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceCastePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceCasteResult result = await _casteService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<CasteModel>>> SearchAsync([FromQuery] SearchCastesParameters parameters, CancellationToken cancellationToken)
  {
    SearchCastesPayload payload = parameters.ToPayload();
    SearchResults<CasteModel> castes = await _casteService.SearchAsync(payload, cancellationToken);
    return Ok(castes);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<CasteModel>> UpdateAsync(Guid id, [FromBody] UpdateCastePayload payload, CancellationToken cancellationToken)
  {
    CasteModel? caste = await _casteService.UpdateAsync(id, payload, cancellationToken);
    return caste is null ? NotFound() : Ok(caste);
  }

  private ActionResult<CasteModel> ToActionResult(CreateOrReplaceCasteResult result)
  {
    CasteModel caste = result.Caste;
    if (result.Created)
    {
      Uri location = new($"{HttpContext.GetBaseUrl()}/castes/{caste.Id}", UriKind.Absolute);
      return Created(location, caste);
    }
    return Ok(caste);
  }
}

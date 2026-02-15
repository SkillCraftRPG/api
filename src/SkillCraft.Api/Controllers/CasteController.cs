using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Castes;
using SkillCraft.Api.Models.Parameters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[Route("castes")]
public class CasteController : ControllerBase
{
  private readonly ICasteService _casteService;

  public CasteController(ICasteService casteService)
  {
    _casteService = casteService;
  }

  [HttpPost]
  [ProducesResponseType<CasteModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<CasteModel>> CreateAsync([FromBody] CreateOrReplaceCastePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceCasteResult result = await _casteService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  [ProducesResponseType<CasteModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<CasteModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    CasteModel? caste = await _casteService.DeleteAsync(id, cancellationToken);
    return caste is null ? NotFound() : Ok(caste);
  }

  [HttpGet("{id}")]
  [ProducesResponseType<CasteModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<CasteModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CasteModel? caste = await _casteService.ReadAsync(id, cancellationToken);
    return caste is null ? NotFound() : Ok(caste);
  }

  [HttpPut("{id}")]
  [ProducesResponseType<CasteModel>(StatusCodes.Status200OK)]
  [ProducesResponseType<CasteModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<CasteModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceCastePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceCasteResult result = await _casteService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  [ProducesResponseType<CasteModel>(StatusCodes.Status200OK)]
  public async Task<ActionResult<SearchResults<CasteModel>>> SearchAsync(SearchCastesParameters parameters, CancellationToken cancellationToken)
  {
    SearchCastesPayload payload = parameters.ToPayload();
    SearchResults<CasteModel> castes = await _casteService.SearchAsync(payload, cancellationToken);
    return Ok(castes);
  }

  [HttpPatch("{id}")]
  [ProducesResponseType<CasteModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
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
      Uri location = new($"{Request.Scheme}://{Request.Host}/castes/{caste.Id}", UriKind.Absolute);
      return Created(location, caste);
    }
    return Ok(caste);
  }
}

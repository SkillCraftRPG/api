using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Specializations;
using SkillCraft.Api.Models.Parameters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[Route("specializations")]
public class SpecializationController : ControllerBase
{
  private readonly ISpecializationService _specializationService;

  public SpecializationController(ISpecializationService specializationService)
  {
    _specializationService = specializationService;
  }

  [HttpPost]
  [ProducesResponseType<SpecializationModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<SpecializationModel>> CreateAsync([FromBody] CreateOrReplaceSpecializationPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpecializationResult result = await _specializationService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  [ProducesResponseType<SpecializationModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<SpecializationModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    SpecializationModel? specialization = await _specializationService.DeleteAsync(id, cancellationToken);
    return specialization is null ? NotFound() : Ok(specialization);
  }

  [HttpGet("{id}")]
  [ProducesResponseType<SpecializationModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<SpecializationModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    SpecializationModel? specialization = await _specializationService.ReadAsync(id, cancellationToken);
    return specialization is null ? NotFound() : Ok(specialization);
  }

  [HttpPut("{id}")]
  [ProducesResponseType<SpecializationModel>(StatusCodes.Status200OK)]
  [ProducesResponseType<SpecializationModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<SpecializationModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceSpecializationPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpecializationResult result = await _specializationService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  [ProducesResponseType<SpecializationModel>(StatusCodes.Status200OK)]
  public async Task<ActionResult<SearchResults<SpecializationModel>>> SearchAsync(SearchSpecializationsParameters parameters, CancellationToken cancellationToken)
  {
    SearchSpecializationsPayload payload = parameters.ToPayload();
    SearchResults<SpecializationModel> specializations = await _specializationService.SearchAsync(payload, cancellationToken);
    return Ok(specializations);
  }

  [HttpPatch("{id}")]
  [ProducesResponseType<SpecializationModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<SpecializationModel>> UpdateAsync(Guid id, [FromBody] UpdateSpecializationPayload payload, CancellationToken cancellationToken)
  {
    SpecializationModel? specialization = await _specializationService.UpdateAsync(id, payload, cancellationToken);
    return specialization is null ? NotFound() : Ok(specialization);
  }

  private ActionResult<SpecializationModel> ToActionResult(CreateOrReplaceSpecializationResult result)
  {
    SpecializationModel specialization = result.Specialization;
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/specializations/{specialization.Id}", UriKind.Absolute);
      return Created(location, specialization);
    }
    return Ok(specialization);
  }
}

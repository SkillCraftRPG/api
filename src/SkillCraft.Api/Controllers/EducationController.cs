using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Educations;
using SkillCraft.Api.Models.Parameters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[Route("educations")]
public class EducationController : ControllerBase
{
  private readonly IEducationService _educationService;

  public EducationController(IEducationService educationService)
  {
    _educationService = educationService;
  }

  [HttpPost]
  [ProducesResponseType<EducationModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<EducationModel>> CreateAsync([FromBody] CreateOrReplaceEducationPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceEducationResult result = await _educationService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  [ProducesResponseType<EducationModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<EducationModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    EducationModel? education = await _educationService.DeleteAsync(id, cancellationToken);
    return education is null ? NotFound() : Ok(education);
  }

  [HttpGet("{id}")]
  [ProducesResponseType<EducationModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<EducationModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    EducationModel? education = await _educationService.ReadAsync(id, cancellationToken);
    return education is null ? NotFound() : Ok(education);
  }

  [HttpPut("{id}")]
  [ProducesResponseType<EducationModel>(StatusCodes.Status200OK)]
  [ProducesResponseType<EducationModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<EducationModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceEducationPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceEducationResult result = await _educationService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  [ProducesResponseType<EducationModel>(StatusCodes.Status200OK)]
  public async Task<ActionResult<SearchResults<EducationModel>>> SearchAsync(SearchEducationsParameters parameters, CancellationToken cancellationToken)
  {
    SearchEducationsPayload payload = parameters.ToPayload();
    SearchResults<EducationModel> educations = await _educationService.SearchAsync(payload, cancellationToken);
    return Ok(educations);
  }

  [HttpPatch("{id}")]
  [ProducesResponseType<EducationModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<EducationModel>> UpdateAsync(Guid id, [FromBody] UpdateEducationPayload payload, CancellationToken cancellationToken)
  {
    EducationModel? education = await _educationService.UpdateAsync(id, payload, cancellationToken);
    return education is null ? NotFound() : Ok(education);
  }

  private ActionResult<EducationModel> ToActionResult(CreateOrReplaceEducationResult result)
  {
    EducationModel education = result.Education;
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/educations/{education.Id}", UriKind.Absolute);
      return Created(location, education);
    }
    return Ok(education);
  }
}

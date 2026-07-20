using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Educations.Models;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Filters;
using SkillCraft.Api.Models.Education;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("educations")]
public class EducationController : ControllerBase
{
  private readonly IEducationService _educationService;

  public EducationController(IEducationService educationService)
  {
    _educationService = educationService;
  }

  [HttpPost]
  public async Task<ActionResult<EducationModel>> CreateAsync([FromBody] CreateOrReplaceEducationPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceEducationResult result = await _educationService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<EducationModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    EducationModel? education = await _educationService.ReadAsync(id, cancellationToken);
    return education is null ? NotFound() : Ok(education);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<EducationModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceEducationPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceEducationResult result = await _educationService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<EducationModel>>> SearchAsync([FromQuery] SearchEducationsParameters parameters, CancellationToken cancellationToken)
  {
    SearchEducationsPayload payload = parameters.ToPayload();
    SearchResults<EducationModel> educations = await _educationService.SearchAsync(payload, cancellationToken);
    return Ok(educations);
  }

  [HttpPatch("{id}")]
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
      Uri location = new($"{HttpContext.GetBaseUrl()}/educations/{education.Id}", UriKind.Absolute);
      return Created(location, education);
    }
    return Ok(education);
  }
}

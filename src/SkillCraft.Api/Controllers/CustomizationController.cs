using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Models.Customization;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("customizations")]
public class CustomizationController : ControllerBase
{
  private readonly ICustomizationService _customizationService;

  public CustomizationController(ICustomizationService customizationService)
  {
    _customizationService = customizationService;
  }

  [HttpPost]
  public async Task<ActionResult<CustomizationModel>> CreateAsync([FromBody] CreateOrReplaceCustomizationPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceCustomizationResult result = await _customizationService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<CustomizationModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CustomizationModel? customization = await _customizationService.ReadAsync(id, cancellationToken);
    return customization is null ? NotFound() : Ok(customization);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<CustomizationModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceCustomizationPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceCustomizationResult result = await _customizationService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<CustomizationModel>>> SearchAsync([FromQuery] SearchCustomizationsParameters parameters, CancellationToken cancellationToken)
  {
    SearchCustomizationsPayload payload = parameters.ToPayload();
    SearchResults<CustomizationModel> customizations = await _customizationService.SearchAsync(payload, cancellationToken);
    return Ok(customizations);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<CustomizationModel>> UpdateAsync(Guid id, [FromBody] UpdateCustomizationPayload payload, CancellationToken cancellationToken)
  {
    CustomizationModel? customization = await _customizationService.UpdateAsync(id, payload, cancellationToken);
    return customization is null ? NotFound() : Ok(customization);
  }

  private ActionResult<CustomizationModel> ToActionResult(CreateOrReplaceCustomizationResult result)
  {
    CustomizationModel customization = result.Customization;
    if (result.Created)
    {
      Uri location = new($"{HttpContext.GetBaseUrl()}/customizations/{customization.Id}", UriKind.Absolute);
      return Created(location, customization);
    }
    return Ok(customization);
  }
}

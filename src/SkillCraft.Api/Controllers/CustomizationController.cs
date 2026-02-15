using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Models.Parameters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[Route("customizations")]
public class CustomizationController : ControllerBase
{
  private readonly ICustomizationService _customizationService;

  public CustomizationController(ICustomizationService customizationService)
  {
    _customizationService = customizationService;
  }

  [HttpPost]
  [ProducesResponseType<CustomizationModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<CustomizationModel>> CreateAsync([FromBody] CreateOrReplaceCustomizationPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceCustomizationResult result = await _customizationService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  [ProducesResponseType<CustomizationModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<CustomizationModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    CustomizationModel? customization = await _customizationService.DeleteAsync(id, cancellationToken);
    return customization is null ? NotFound() : Ok(customization);
  }

  [HttpGet("{id}")]
  [ProducesResponseType<CustomizationModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<CustomizationModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CustomizationModel? customization = await _customizationService.ReadAsync(id, cancellationToken);
    return customization is null ? NotFound() : Ok(customization);
  }

  [HttpPut("{id}")]
  [ProducesResponseType<CustomizationModel>(StatusCodes.Status200OK)]
  [ProducesResponseType<CustomizationModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<CustomizationModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceCustomizationPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceCustomizationResult result = await _customizationService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  [ProducesResponseType<CustomizationModel>(StatusCodes.Status200OK)]
  public async Task<ActionResult<SearchResults<CustomizationModel>>> SearchAsync(SearchCustomizationsParameters parameters, CancellationToken cancellationToken)
  {
    SearchCustomizationsPayload payload = parameters.ToPayload();
    SearchResults<CustomizationModel> customizations = await _customizationService.SearchAsync(payload, cancellationToken);
    return Ok(customizations);
  }

  [HttpPatch("{id}")]
  [ProducesResponseType<CustomizationModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
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
      Uri location = new($"{Request.Scheme}://{Request.Host}/customizations/{customization.Id}", UriKind.Absolute);
      return Created(location, customization);
    }
    return Ok(customization);
  }
}

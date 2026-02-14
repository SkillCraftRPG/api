using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Worlds;
using SkillCraft.Api.Models.Parameters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[Route("worlds")]
public class WorldController : ControllerBase
{
  private readonly IWorldService _worldService;

  public WorldController(IWorldService worldService)
  {
    _worldService = worldService;
  }

  [HttpPost]
  [ProducesResponseType<WorldModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<WorldModel>> CreateAsync([FromBody] CreateOrReplaceWorldPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceWorldResult result = await _worldService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  [ProducesResponseType<WorldModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<WorldModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    WorldModel? world = await _worldService.DeleteAsync(id, cancellationToken);
    return world is null ? NotFound() : Ok(world);
  }

  [HttpGet("{id}")]
  [ProducesResponseType<WorldModel>(StatusCodes.Status200OK)]
  public async Task<ActionResult<WorldModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    WorldModel? world = await _worldService.ReadAsync(id, cancellationToken);
    return world is null ? NotFound() : Ok(world);
  }

  [HttpPut("{id}")]
  [ProducesResponseType<WorldModel>(StatusCodes.Status200OK)]
  [ProducesResponseType<WorldModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<WorldModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceWorldPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceWorldResult result = await _worldService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  [ProducesResponseType<WorldModel>(StatusCodes.Status200OK)]
  public async Task<ActionResult<SearchResults<WorldModel>>> SearchAsync(SearchWorldsParameters parameters, CancellationToken cancellationToken)
  {
    SearchWorldsPayload payload = parameters.ToPayload();
    SearchResults<WorldModel> worlds = await _worldService.SearchAsync(payload, cancellationToken);
    return Ok(worlds);
  }

  [HttpPatch("{id}")]
  [ProducesResponseType<WorldModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<WorldModel>> UpdateAsync(Guid id, [FromBody] UpdateWorldPayload payload, CancellationToken cancellationToken)
  {
    WorldModel? world = await _worldService.UpdateAsync(id, payload, cancellationToken);
    return world is null ? NotFound() : Ok(world);
  }

  private ActionResult<WorldModel> ToActionResult(CreateOrReplaceWorldResult result)
  {
    WorldModel world = result.World;
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/worlds/{world.Id}", UriKind.Absolute);
      return Created(location, world);
    }
    return Ok(world);
  }
}

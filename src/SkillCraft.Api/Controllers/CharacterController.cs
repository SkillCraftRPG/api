using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Characters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[Route("characters")]
public class CharacterController : ControllerBase
{
  private readonly ICharacterService _characterService;

  public CharacterController(ICharacterService characterService)
  {
    _characterService = characterService;
  }

  [HttpPost]
  [ProducesResponseType<CharacterModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<CharacterModel>> CreateAsync([FromBody] CreateCharacterPayload payload, CancellationToken cancellationToken)
  {
    CharacterModel character = await _characterService.CreateOrReplaceAsync(payload, cancellationToken);
    Uri location = new($"{Request.Scheme}://{Request.Host}/characters/{character.Id}", UriKind.Absolute);
    return Created(location, character);
  }

  [HttpDelete("{id}")]
  [ProducesResponseType<CharacterModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<CharacterModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _characterService.DeleteAsync(id, cancellationToken);
    return character is null ? NotFound() : Ok(character);
  }

  [HttpGet("{id}")]
  [ProducesResponseType<CharacterModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<CharacterModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _characterService.ReadAsync(id, cancellationToken);
    return character is null ? NotFound() : Ok(character);
  }
}

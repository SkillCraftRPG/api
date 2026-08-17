using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Filters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("characters/{characterId}/modifiers")]
public class CharacterModifierController : ControllerBase
{
  private readonly ICharacterModifierService _characterModifierService;

  public CharacterModifierController(ICharacterModifierService characterModifierService)
  {
    _characterModifierService = characterModifierService;
  }

  [HttpPost]
  public async Task<ActionResult<CharacterModel>> CreateAsync(Guid characterId, [FromBody] CreateOrReplaceCharacterModifierPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceCharacterModifierResult? result = await _characterModifierService.CreateOrReplaceAsync(characterId, payload, modifierId: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{modifierId}")]
  public async Task<ActionResult<CharacterModel>> RemoveAsync(Guid characterId, Guid modifierId, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _characterModifierService.RemoveAsync(characterId, modifierId, cancellationToken);
    return character is null ? NotFound() : Ok(character);
  }

  [HttpPut("{modifierId}")]
  public async Task<ActionResult<CharacterModel>> ReplaceAsync(Guid characterId, Guid modifierId, [FromBody] CreateOrReplaceCharacterModifierPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceCharacterModifierResult? result = await _characterModifierService.CreateOrReplaceAsync(characterId, payload, modifierId, cancellationToken);
    return ToActionResult(result);
  }

  private ActionResult<CharacterModel> ToActionResult(CreateOrReplaceCharacterModifierResult? result)
  {
    if (result is null)
    {
      return NotFound();
    }

    CharacterModel character = result.Character;
    if (result.Created)
    {
      Uri location = new($"{HttpContext.GetBaseUrl()}/characters/{character.Id}", UriKind.Absolute);
      return Created(location, character);
    }
    return Ok(character);
  }
}

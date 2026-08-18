using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Filters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("characters/{characterId}/customizations/{customizationId}")]
public class CharacterCustomizationController : ControllerBase
{
  private readonly ICharacterCustomizationService _characterCustomizationService;

  public CharacterCustomizationController(ICharacterCustomizationService characterCustomizationService)
  {
    _characterCustomizationService = characterCustomizationService;
  }

  [HttpPut]
  public async Task<ActionResult<CharacterModel>> AddAsync(Guid characterId, Guid customizationId, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _characterCustomizationService.AddAsync(characterId, customizationId, cancellationToken);
    return character is null ? NotFound() : Ok(character);
  }
}

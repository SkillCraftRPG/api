using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Filters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("characters/{characterId}/languages/{languageId}")]
public class CharacterLanguageController : ControllerBase
{
  private readonly ICharacterLanguageService _characterLanguageService;

  public CharacterLanguageController(ICharacterLanguageService characterLanguageService)
  {
    _characterLanguageService = characterLanguageService;
  }

  [HttpDelete]
  public async Task<ActionResult<CharacterModel>> RemoveAsync(Guid characterId, Guid languageId, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _characterLanguageService.RemoveAsync(characterId, languageId, cancellationToken);
    return character is null ? NotFound() : Ok(character);
  }
}

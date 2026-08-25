using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Filters;
using SkillCraft.Api.Models.Character;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("characters")]
public class CharacterController : ControllerBase
{
  private readonly ICharacterService _characterService;

  public CharacterController(ICharacterService characterService)
  {
    _characterService = characterService;
  }

  [HttpPost]
  public async Task<ActionResult<CharacterModel>> CreateAsync([FromBody] CreateCharacterPayload payload, CancellationToken cancellationToken)
  {
    CharacterModel character = await _characterService.CreateAsync(payload, cancellationToken);
    Uri location = new($"{HttpContext.GetBaseUrl()}/characters/{character.Id}", UriKind.Absolute);
    return Created(location, character);
  }

  [HttpPost("{id}/experience")]
  public async Task<ActionResult<CharacterModel>> GainExperienceAsync(Guid id, [FromBody] GainCharacterExperiencePayload payload, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _characterService.GainExperienceAsync(id, payload, cancellationToken);
    return character is null ? NotFound() : Ok(character);
  }

  [HttpPost("{id}/attributes")]
  public async Task<ActionResult<CharacterModel>> IncreaseAttributesAsync(Guid id, [FromBody] IncreaseCharacterAttributesPayload payload, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _characterService.IncreaseAttributesAsync(id, payload, cancellationToken);
    return character is null ? NotFound() : Ok(character);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<CharacterModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _characterService.ReadAsync(id, cancellationToken);
    return character is null ? NotFound() : Ok(character);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<CharacterModel>>> SearchAsync([FromQuery] SearchCharactersParameters parameters, CancellationToken cancellationToken)
  {
    SearchCharactersPayload payload = parameters.ToPayload();
    SearchResults<CharacterModel> characters = await _characterService.SearchAsync(payload, cancellationToken);
    return Ok(characters);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<CharacterModel>> UpdateAsync(Guid id, [FromBody] UpdateCharacterPayload payload, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _characterService.UpdateAsync(id, payload, cancellationToken);
    return character is null ? NotFound() : Ok(character);
  }
}

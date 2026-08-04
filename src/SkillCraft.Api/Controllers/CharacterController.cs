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
}

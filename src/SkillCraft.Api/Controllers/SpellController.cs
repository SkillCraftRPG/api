using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Spells;
using SkillCraft.Api.Core.Spells.Models;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Filters;
using SkillCraft.Api.Models.Spell;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("spells")]
public class SpellController : ControllerBase
{
  private readonly ISpellService _spellService;

  public SpellController(ISpellService spellService)
  {
    _spellService = spellService;
  }

  [HttpPost]
  public async Task<ActionResult<SpellModel>> CreateAsync([FromBody] CreateOrReplaceSpellPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpellResult result = await _spellService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<SpellModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    SpellModel? spell = await _spellService.ReadAsync(id, cancellationToken);
    return spell is null ? NotFound() : Ok(spell);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<SpellModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceSpellPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpellResult result = await _spellService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<SpellModel>>> SearchAsync([FromQuery] SearchSpellsParameters parameters, CancellationToken cancellationToken)
  {
    SearchSpellsPayload payload = parameters.ToPayload();
    SearchResults<SpellModel> spells = await _spellService.SearchAsync(payload, cancellationToken);
    return Ok(spells);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<SpellModel>> UpdateAsync(Guid id, [FromBody] UpdateSpellPayload payload, CancellationToken cancellationToken)
  {
    SpellModel? spell = await _spellService.UpdateAsync(id, payload, cancellationToken);
    return spell is null ? NotFound() : Ok(spell);
  }

  private ActionResult<SpellModel> ToActionResult(CreateOrReplaceSpellResult result)
  {
    SpellModel spell = result.Spell;
    if (result.Created)
    {
      Uri location = new($"{HttpContext.GetBaseUrl()}/spells/{spell.Id}", UriKind.Absolute);
      return Created(location, spell);
    }
    return Ok(spell);
  }
}

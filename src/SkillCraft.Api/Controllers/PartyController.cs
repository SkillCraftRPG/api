using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Parties;
using SkillCraft.Api.Models.Parameters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[Route("parties")]
public class PartyController : ControllerBase
{
  private readonly IPartyService _partyService;

  public PartyController(IPartyService partyService)
  {
    _partyService = partyService;
  }

  [HttpPost]
  [ProducesResponseType<PartyModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<PartyModel>> CreateAsync([FromBody] CreateOrReplacePartyPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplacePartyResult result = await _partyService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  [ProducesResponseType<PartyModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<PartyModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    PartyModel? party = await _partyService.DeleteAsync(id, cancellationToken);
    return party is null ? NotFound() : Ok(party);
  }

  [HttpGet("{id}")]
  [ProducesResponseType<PartyModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<PartyModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    PartyModel? party = await _partyService.ReadAsync(id, cancellationToken);
    return party is null ? NotFound() : Ok(party);
  }

  [HttpPut("{id}")]
  [ProducesResponseType<PartyModel>(StatusCodes.Status200OK)]
  [ProducesResponseType<PartyModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<PartyModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplacePartyPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplacePartyResult result = await _partyService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  [ProducesResponseType<PartyModel>(StatusCodes.Status200OK)]
  public async Task<ActionResult<SearchResults<PartyModel>>> SearchAsync(SearchPartiesParameters parameters, CancellationToken cancellationToken)
  {
    SearchPartiesPayload payload = parameters.ToPayload();
    SearchResults<PartyModel> parties = await _partyService.SearchAsync(payload, cancellationToken);
    return Ok(parties);
  }

  [HttpPatch("{id}")]
  [ProducesResponseType<PartyModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<PartyModel>> UpdateAsync(Guid id, [FromBody] UpdatePartyPayload payload, CancellationToken cancellationToken)
  {
    PartyModel? party = await _partyService.UpdateAsync(id, payload, cancellationToken);
    return party is null ? NotFound() : Ok(party);
  }

  private ActionResult<PartyModel> ToActionResult(CreateOrReplacePartyResult result)
  {
    PartyModel party = result.Party;
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/parties/{party.Id}", UriKind.Absolute);
      return Created(location, party);
    }
    return Ok(party);
  }
}

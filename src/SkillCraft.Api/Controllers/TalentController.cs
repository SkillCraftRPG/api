using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Contracts.Talents;
using SkillCraft.Api.Models.Parameters;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[Route("talents")]
public class TalentController : ControllerBase
{
  private readonly ITalentService _talentService;

  public TalentController(ITalentService talentService)
  {
    _talentService = talentService;
  }

  [HttpPost]
  [ProducesResponseType<TalentModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<TalentModel>> CreateAsync([FromBody] CreateOrReplaceTalentPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceTalentResult result = await _talentService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  [ProducesResponseType<TalentModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<TalentModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    TalentModel? talent = await _talentService.DeleteAsync(id, cancellationToken);
    return talent is null ? NotFound() : Ok(talent);
  }

  [HttpGet("{id}")]
  [ProducesResponseType<TalentModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<TalentModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    TalentModel? talent = await _talentService.ReadAsync(id, cancellationToken);
    return talent is null ? NotFound() : Ok(talent);
  }

  [HttpPut("{id}")]
  [ProducesResponseType<TalentModel>(StatusCodes.Status200OK)]
  [ProducesResponseType<TalentModel>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<ActionResult<TalentModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceTalentPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceTalentResult result = await _talentService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  [ProducesResponseType<TalentModel>(StatusCodes.Status200OK)]
  public async Task<ActionResult<SearchResults<TalentModel>>> SearchAsync(SearchTalentsParameters parameters, CancellationToken cancellationToken)
  {
    SearchTalentsPayload payload = parameters.ToPayload();
    SearchResults<TalentModel> talents = await _talentService.SearchAsync(payload, cancellationToken);
    return Ok(talents);
  }

  [HttpPatch("{id}")]
  [ProducesResponseType<TalentModel>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<TalentModel>> UpdateAsync(Guid id, [FromBody] UpdateTalentPayload payload, CancellationToken cancellationToken)
  {
    TalentModel? talent = await _talentService.UpdateAsync(id, payload, cancellationToken);
    return talent is null ? NotFound() : Ok(talent);
  }

  private ActionResult<TalentModel> ToActionResult(CreateOrReplaceTalentResult result)
  {
    TalentModel talent = result.Talent;
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/talents/{talent.Id}", UriKind.Absolute);
      return Created(location, talent);
    }
    return Ok(talent);
  }
}

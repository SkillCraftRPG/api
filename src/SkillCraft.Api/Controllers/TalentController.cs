using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Talents.Models;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Filters;
using SkillCraft.Api.Models.Talent;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("talents")]
public class TalentController : ControllerBase
{
  private readonly ITalentService _talentService;

  public TalentController(ITalentService talentService)
  {
    _talentService = talentService;
  }

  [HttpPost]
  public async Task<ActionResult<TalentModel>> CreateAsync([FromBody] CreateOrReplaceTalentPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceTalentResult result = await _talentService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TalentModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    TalentModel? talent = await _talentService.ReadAsync(id, cancellationToken);
    return talent is null ? NotFound() : Ok(talent);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<TalentModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceTalentPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceTalentResult result = await _talentService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<TalentModel>>> SearchAsync([FromQuery] SearchTalentsParameters parameters, CancellationToken cancellationToken)
  {
    SearchTalentsPayload payload = parameters.ToPayload();
    SearchResults<TalentModel> talents = await _talentService.SearchAsync(payload, cancellationToken);
    return Ok(talents);
  }

  [HttpPatch("{id}")]
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
      Uri location = new($"{HttpContext.GetBaseUrl()}/talents/{talent.Id}", UriKind.Absolute);
      return Created(location, talent);
    }
    return Ok(talent);
  }
}

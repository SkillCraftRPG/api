using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Identity;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Models.Session;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[Route("sessions")]
public class SessionController : ControllerBase
{
  private readonly ISessionGateway _sessionGateway;

  public SessionController(ISessionGateway sessionGateway)
  {
    _sessionGateway = sessionGateway;
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<SessionModel>>> SearchAsync(CancellationToken cancellationToken)
  {
    User user = HttpContext.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");
    IReadOnlyCollection<Session> sessions = await _sessionGateway.ListAsync(user, cancellationToken);

    SessionMapper mapper = new(HttpContext.GetSessionId());
    SearchResults<SessionModel> results = new(sessions.Select(mapper.Map));
    return Ok(results);
  }
}

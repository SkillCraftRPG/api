using Krakenar.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Models.Account;

namespace SkillCraft.Api.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
  [HttpGet("profile")]
  [Authorize]
  [ProducesResponseType<CurrentUser>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public ActionResult<CurrentUser> GetProfile()
  {
    User user = HttpContext.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");
    CurrentUser profile = new(user);
    return Ok(profile);
  }
}

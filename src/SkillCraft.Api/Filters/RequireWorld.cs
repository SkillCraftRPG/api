using Krakenar.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SkillCraft.Api.Constants;
using SkillCraft.Api.Core.Worlds.Models;
using SkillCraft.Api.Extensions;

namespace SkillCraft.Api.Filters;

internal class RequireWorld : ActionFilterAttribute
{
  public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    HttpContext httpContext = context.HttpContext;
    WorldModel? world = httpContext.GetWorld();
    if (world is null)
    {
      Error error = new(code: "MissingWorld", message: "A world is required.");
      error.Data["Header"] = Headers.World;

      ProblemDetailsFactory problemDetailsFactory = httpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
      ProblemDetails problemDetails = problemDetailsFactory.CreateProblemDetails(httpContext, StatusCodes.Status400BadRequest, error);

      IProblemDetailsService problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
      ProblemDetailsContext problemDetailsContext = new()
      {
        HttpContext = httpContext,
        ProblemDetails = problemDetails
      };
      await problemDetailsService.WriteAsync(problemDetailsContext);
    }
    else
    {
      await next();
    }
  }
}

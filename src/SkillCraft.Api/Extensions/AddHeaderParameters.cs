using Microsoft.OpenApi;
using SkillCraft.Api.Middlewares;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SkillCraft.Api.Extensions;

internal class AddHeaderParameters : IOperationFilter
{
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    operation.Parameters?.Add(new OpenApiParameter
    {
      In = ParameterLocation.Header,
      Name = ResolveWorld.Header,
      Description = "Enter your wworld ID in the input below:"
    });
  }
}

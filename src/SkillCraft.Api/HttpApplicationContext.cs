using Krakenar.Contracts;
using Krakenar.Contracts.Users;
using SkillCraft.Api.Core;
using SkillCraft.Api.Extensions;
using SkillCraft.Api.Infrastructure;

namespace SkillCraft.Api;

internal class HttpApplicationContext : IContext
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private HttpContext Context => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("The HttpContext is required.");

  public HttpApplicationContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }


  public Guid UserId
  {
    get
    {
      User user = Context.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");
      return user.Id;
    }
  }

  public IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes() => Context.GetSessionCustomAttributes();

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
  {
    GameContext database = Context.RequestServices.GetRequiredService<GameContext>();
    return await database.SaveChangesAsync(cancellationToken);
  }
}

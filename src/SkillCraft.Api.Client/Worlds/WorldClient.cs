using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Worlds;

namespace SkillCraft.Api.Client.Worlds;

internal class WorldClient : IWorldService // TODO(fpion): WorldClient with RequestContext
{
  public async Task<CreateOrReplaceWorldResult> CreateOrReplaceAsync(CreateOrReplaceWorldPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    return new CreateOrReplaceWorldResult(new WorldModel(), Created: false); // TODO(fpion): implement
  }

  public async Task<WorldModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return null; // TODO(fpion): implement
  }

  public async Task<WorldModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return null; // TODO(fpion): implement
  }

  public async Task<SearchResults<WorldModel>> SearchAsync(SearchWorldsPayload payload, CancellationToken cancellationToken)
  {
    return new SearchResults<WorldModel>(); // TODO(fpion): implement
  }

  public async Task<WorldModel?> UpdateAsync(Guid id, UpdateWorldPayload payload, CancellationToken cancellationToken)
  {
    return null; // TODO(fpion): implement
  }
}

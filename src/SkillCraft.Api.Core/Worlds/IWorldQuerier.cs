using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Worlds;

namespace SkillCraft.Api.Core.Worlds;

public interface IWorldQuerier
{
  Task<int> CountAsync(CancellationToken cancellationToken = default);

  Task<WorldModel> ReadAsync(World world, CancellationToken cancellationToken = default);
  Task<WorldModel?> ReadAsync(WorldId id, CancellationToken cancellationToken = default);
  Task<WorldModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<WorldModel>> SearchAsync(SearchWorldsPayload payload, CancellationToken cancellationToken = default);
}

using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Worlds.Events;
using SkillCraft.Api.Core.Worlds.Models;

namespace SkillCraft.Api.Core.Worlds;

public interface IWorldRepository
{
  void Add(World world);
  void Remove(World world);
  void Update(World world, WorldUpdated record);

  Task<int> CountAsync(CancellationToken cancellationToken = default);

  Task EnsureUnicityAsync(World world, CancellationToken cancellationToken = default);

  Task<World?> LoadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<WorldModel> ReadAsync(World world, CancellationToken cancellationToken = default);
  Task<WorldModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<WorldModel?> ReadAsync(string key, CancellationToken cancellationToken = default);

  Task<SearchResults<WorldModel>> SearchAsync(SearchWorldsPayload payload, CancellationToken cancellationToken = default);
}

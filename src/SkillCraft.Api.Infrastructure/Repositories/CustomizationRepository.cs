using Logitar.EventSourcing;
using SkillCraft.Api.Core.Customizations;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class CustomizationRepository : Repository, ICustomizationRepository
{
  public CustomizationRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Customization?> LoadAsync(CustomizationId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Customization>(id.StreamId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Customization>> LoadAsync(IEnumerable<CustomizationId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Customization>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public async Task SaveAsync(Customization customization, CancellationToken cancellationToken)
  {
    await base.SaveAsync(customization, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Customization> customizations, CancellationToken cancellationToken)
  {
    await base.SaveAsync(customizations, cancellationToken);
  }
}

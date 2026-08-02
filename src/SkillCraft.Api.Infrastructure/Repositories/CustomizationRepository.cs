using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class CustomizationRepository : Repository, ICustomizationRepository
{
  private readonly GameContext _database;

  public CustomizationRepository(GameContext database, IEventStore eventStore) : base(eventStore)
  {
    _database = database;
  }

  public async Task<Customization?> LoadAsync(CustomizationId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Customization>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Customization>> LoadAsync(IEnumerable<CustomizationId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Customization>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(Customization customization, CancellationToken cancellationToken)
  {
    await base.SaveAsync(customization, cancellationToken);

    await SynchronizeAsync(customization, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Customization> customizations, CancellationToken cancellationToken)
  {
    await base.SaveAsync(customizations, cancellationToken);

    foreach (Customization customization in customizations)
    {
      await SynchronizeAsync(customization, cancellationToken);
    }
  }

  private async Task SynchronizeAsync(Customization customization, CancellationToken cancellationToken)
  {
    CustomizationEntity? entity = await _database.Customizations.SingleOrDefaultAsync(x => x.StreamId == customization.Id.Value, cancellationToken);
    if (entity is null)
    {
      entity = new CustomizationEntity(customization);
      _database.Customizations.Add(entity);
    }
    else
    {
      entity.Update(customization);
    }
    await _database.SaveChangesAsync(cancellationToken);
  }
}

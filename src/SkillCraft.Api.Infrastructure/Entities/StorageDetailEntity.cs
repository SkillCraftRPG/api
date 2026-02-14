using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Storages.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class StorageDetailEntity
{
  public int StorageDetailId { get; private set; }
  public string Key { get; private set; } = string.Empty;

  public StorageSummaryEntity? Summary { get; private set; }
  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }
  public Guid WorldUid { get; private set; }

  public string EntityKind { get; private set; } = string.Empty;
  public Guid EntityId { get; private set; }

  public long Size { get; private set; }

  public StorageDetailEntity(StorageSummaryEntity summary, EntityStored @event)
  {
    Entity entity = Entity.Parse(@event.Key);

    Summary = summary;
    World = summary.World;
    WorldId = summary.WorldId;
    WorldUid = summary.WorldUid;

    EntityKind = entity.Kind;
    EntityId = entity.Id;

    Size = @event.Size;
  }

  private StorageDetailEntity()
  {
  }

  public void Update(EntityStored @event)
  {
    Size = @event.Size;
  }
}

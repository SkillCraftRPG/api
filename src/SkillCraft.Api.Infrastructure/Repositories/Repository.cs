using SkillCraft.Api.Core;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal abstract class Repository
{
  protected virtual GameContext Database { get; set; }

  protected Repository(GameContext database)
  {
    Database = database;
  }

  protected virtual void RecordChange(ChangeEvent @event)
  {
    HistoryRecord record = new(@event);
    Database.History.Add(record);
  }
}

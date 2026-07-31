namespace SkillCraft.Api.Core.Items.Events;

public class ItemDeleted : DeleteEvent
{
  public ItemDeleted() : base()
  {
  }

  public ItemDeleted(Item item, Guid userId) : base(item, userId)
  {
  }
}

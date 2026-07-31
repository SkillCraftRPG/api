namespace SkillCraft.Api.Core.Lineages.Events;

public class LineageDeleted : DeleteEvent
{
  public LineageDeleted() : base()
  {
  }

  public LineageDeleted(Lineage lineage, Guid userId) : base(lineage, userId)
  {
  }
}

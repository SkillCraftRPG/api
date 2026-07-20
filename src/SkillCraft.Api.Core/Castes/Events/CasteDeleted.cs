namespace SkillCraft.Api.Core.Castes.Events;

public class CasteDeleted : DeleteEvent
{
  public CasteDeleted() : base()
  {
  }

  public CasteDeleted(Caste caste, Guid userId) : base(caste, userId)
  {
  }
}

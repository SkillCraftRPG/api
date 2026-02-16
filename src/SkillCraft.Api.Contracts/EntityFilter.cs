namespace SkillCraft.Api.Contracts;

public record EntityFilter
{
  public Guid? Id { get; set; }

  public EntityFilter(Guid? id = null)
  {
    Id = id;
  }
}

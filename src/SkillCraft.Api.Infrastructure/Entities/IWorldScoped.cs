namespace SkillCraft.Api.Infrastructure.Entities;

public interface IWorldScoped
{
  int WorldId { get; }
  Guid WorldUid { get; }
}

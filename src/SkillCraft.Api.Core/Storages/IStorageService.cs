namespace SkillCraft.Api.Core.Storages;

public interface IStorageService
{
  Task ExecuteWithQuotaAsync(IEntityProvider resource, Func<Task>? function = null, CancellationToken cancellationToken = default);
}

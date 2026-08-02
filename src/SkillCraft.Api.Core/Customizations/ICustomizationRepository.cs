namespace SkillCraft.Api.Core.Customizations;

public interface ICustomizationRepository
{
  Task<Customization?> LoadAsync(CustomizationId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Customization>> LoadAsync(IEnumerable<CustomizationId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Customization customization, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Customization> customizations, CancellationToken cancellationToken = default);
}

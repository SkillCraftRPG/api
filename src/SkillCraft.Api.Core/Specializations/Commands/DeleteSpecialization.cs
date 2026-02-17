using Logitar.CQRS;
using SkillCraft.Api.Contracts.Specializations;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Specializations.Commands;

internal record DeleteSpecializationCommand(Guid Id) : ICommand<SpecializationModel?>;

internal class DeleteSpecializationCommandHandler : ICommandHandler<DeleteSpecializationCommand, SpecializationModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ISpecializationQuerier _specializationQuerier;
  private readonly ISpecializationRepository _specializationRepository;
  private readonly IStorageService _storageService;

  public DeleteSpecializationCommandHandler(
    IContext context,
    IPermissionService permissionService,
    ISpecializationQuerier specializationQuerier,
    ISpecializationRepository specializationRepository,
    IStorageService storageService)
  {
    _context = context;
    _permissionService = permissionService;
    _specializationQuerier = specializationQuerier;
    _specializationRepository = specializationRepository;
    _storageService = storageService;
  }

  public async Task<SpecializationModel?> HandleAsync(DeleteSpecializationCommand command, CancellationToken cancellationToken)
  {
    SpecializationId specializationId = new(command.Id, _context.WorldId);
    Specialization? specialization = await _specializationRepository.LoadAsync(specializationId, cancellationToken);
    if (specialization is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Delete, specialization, cancellationToken);
    SpecializationModel model = await _specializationQuerier.ReadAsync(specialization, cancellationToken);

    specialization.Delete(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      specialization,
      async () => await _specializationRepository.SaveAsync(specialization, cancellationToken),
      cancellationToken);

    return model;
  }
}

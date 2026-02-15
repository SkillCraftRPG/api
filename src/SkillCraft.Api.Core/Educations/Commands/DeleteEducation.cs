using Logitar.CQRS;
using SkillCraft.Api.Contracts.Educations;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Educations.Commands;

internal record DeleteEducationCommand(Guid Id) : ICommand<EducationModel?>;

internal class DeleteEducationCommandHandler : ICommandHandler<DeleteEducationCommand, EducationModel?>
{
  private readonly IContext _context;
  private readonly IEducationQuerier _educationQuerier;
  private readonly IEducationRepository _educationRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public DeleteEducationCommandHandler(
    IContext context,
    IEducationQuerier educationQuerier,
    IEducationRepository educationRepository,
    IPermissionService permissionService,
    IStorageService storageService)
  {
    _context = context;
    _educationQuerier = educationQuerier;
    _educationRepository = educationRepository;
    _permissionService = permissionService;
    _storageService = storageService;
  }

  public async Task<EducationModel?> HandleAsync(DeleteEducationCommand command, CancellationToken cancellationToken)
  {
    EducationId educationId = new(command.Id, _context.WorldId);
    Education? education = await _educationRepository.LoadAsync(educationId, cancellationToken);
    if (education is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Delete, education, cancellationToken);
    EducationModel model = await _educationQuerier.ReadAsync(education, cancellationToken);

    education.Delete(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      education,
      async () => await _educationRepository.SaveAsync(education, cancellationToken),
      cancellationToken);

    return model;
  }
}

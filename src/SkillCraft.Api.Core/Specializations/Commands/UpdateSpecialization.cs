using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Specializations;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Specializations.Validators;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Specializations.Commands;

internal record UpdateSpecializationCommand(Guid Id, UpdateSpecializationPayload Payload) : ICommand<SpecializationModel?>;

internal class UpdateSpecializationCommandHandler : ICommandHandler<UpdateSpecializationCommand, SpecializationModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ISpecializationQuerier _specializationQuerier;
  private readonly ISpecializationRepository _specializationRepository;
  private readonly IStorageService _storageService;

  public UpdateSpecializationCommandHandler(
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

  public async Task<SpecializationModel?> HandleAsync(UpdateSpecializationCommand command, CancellationToken cancellationToken)
  {
    UpdateSpecializationPayload payload = command.Payload;
    new UpdateSpecializationValidator().ValidateAndThrow(payload);

    SpecializationId specializationId = new(command.Id, _context.WorldId);
    Specialization? specialization = await _specializationRepository.LoadAsync(specializationId, cancellationToken);
    if (specialization is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, specialization, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      specialization.Name = new Name(payload.Name);
    }
    if (payload.Summary is not null)
    {
      specialization.Summary = Summary.TryCreate(payload.Summary.Value);
    }
    if (payload.Description is not null)
    {
      specialization.Description = Description.TryCreate(payload.Description.Value);
    }

    // TODO(fpion): Requirements { Talent, Other }
    // TODO(fpion): Options { Talents, Other }
    // TODO(fpion): Doctrine { Name, Description, DiscountedTalents, Features }

    specialization.Update(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      specialization,
      async () => await _specializationRepository.SaveAsync(specialization, cancellationToken),
      cancellationToken);

    return await _specializationQuerier.ReadAsync(specialization, cancellationToken);
  }
}

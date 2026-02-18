using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Specializations;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Specializations.Validators;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Specializations.Commands;

internal record UpdateSpecializationCommand(Guid Id, UpdateSpecializationPayload Payload) : ICommand<SpecializationModel?>;

internal class UpdateSpecializationCommandHandler : SaveSpecialization, ICommandHandler<UpdateSpecializationCommand, SpecializationModel?>
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
    IStorageService storageService,
    ITalentRepository talentRepository) : base(talentRepository)
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

    WorldId worldId = _context.WorldId;

    SpecializationId specializationId = new(command.Id, worldId);
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

    IReadOnlyDictionary<Guid, Talent> talents = await LoadTalentsAsync(payload.Requirements, payload.Options, worldId, cancellationToken);
    if (payload.Requirements is not null)
    {
      SetRequirements(specialization, payload.Requirements, talents);
    }
    if (payload.Options is not null)
    {
      SetOptions(specialization, payload.Options, talents);
    }
    // TODO(fpion): Doctrine { Name, Description, DiscountedTalents, Features }

    specialization.Update(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      specialization,
      async () => await _specializationRepository.SaveAsync(specialization, cancellationToken),
      cancellationToken);

    return await _specializationQuerier.ReadAsync(specialization, cancellationToken);
  }
}

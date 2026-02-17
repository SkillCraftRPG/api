using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Specializations;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Specializations.Validators;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Specializations.Commands;

internal record CreateOrReplaceSpecializationCommand(CreateOrReplaceSpecializationPayload Payload, Guid? Id) : ICommand<CreateOrReplaceSpecializationResult>;

internal class CreateOrReplaceSpecializationCommandHandler : ICommandHandler<CreateOrReplaceSpecializationCommand, CreateOrReplaceSpecializationResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly ISpecializationQuerier _specializationQuerier;
  private readonly ISpecializationRepository _specializationRepository;
  private readonly IStorageService _storageService;

  public CreateOrReplaceSpecializationCommandHandler(
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

  public async Task<CreateOrReplaceSpecializationResult> HandleAsync(CreateOrReplaceSpecializationCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpecializationPayload payload = command.Payload;
    new CreateOrReplaceSpecializationValidator().ValidateAndThrow(payload);

    UserId userId = _context.UserId;
    WorldId worldId = _context.WorldId;

    SpecializationId specializationId = SpecializationId.NewId(worldId);
    Specialization? specialization = null;
    if (command.Id.HasValue)
    {
      specializationId = new(command.Id.Value, worldId);
      specialization = await _specializationRepository.LoadAsync(specializationId, cancellationToken);
    }

    Name name = new(payload.Name);
    Tier tier = new(payload.Tier);

    bool created = false;
    if (specialization is null)
    {
      await _permissionService.CheckAsync(Actions.CreateSpecialization, cancellationToken);

      specialization = new Specialization(worldId, tier, name, userId, specializationId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, specialization, cancellationToken);

      specialization.Name = name;
    }

    specialization.Summary = Summary.TryCreate(payload.Summary);
    specialization.Description = Description.TryCreate(payload.Description);

    // TODO(fpion): Requirements { Talent, Other }
    // TODO(fpion): Options { Talents, Other }
    // TODO(fpion): Doctrine { Name, Description, DiscountedTalents, Features }

    specialization.Update(userId);

    await _storageService.ExecuteWithQuotaAsync(
      specialization,
      async () => await _specializationRepository.SaveAsync(specialization, cancellationToken),
      cancellationToken);

    SpecializationModel model = await _specializationQuerier.ReadAsync(specialization, cancellationToken);
    return new CreateOrReplaceSpecializationResult(model, created);
  }
}

using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Educations;
using SkillCraft.Api.Core.Educations.Validators;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Educations.Commands;

internal record CreateOrReplaceEducationCommand(CreateOrReplaceEducationPayload Payload, Guid? Id) : ICommand<CreateOrReplaceEducationResult>;

internal class CreateOrReplaceEducationCommandHandler : ICommandHandler<CreateOrReplaceEducationCommand, CreateOrReplaceEducationResult>
{
  private readonly IContext _context;
  private readonly IEducationQuerier _educationQuerier;
  private readonly IEducationRepository _educationRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public CreateOrReplaceEducationCommandHandler(
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

  public async Task<CreateOrReplaceEducationResult> HandleAsync(CreateOrReplaceEducationCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceEducationPayload payload = command.Payload;
    new CreateOrReplaceEducationValidator().ValidateAndThrow(payload);

    UserId userId = _context.UserId;
    WorldId worldId = _context.WorldId;

    EducationId educationId = EducationId.NewId(worldId);
    Education? education = null;
    if (command.Id.HasValue)
    {
      educationId = new(command.Id.Value, worldId);
      education = await _educationRepository.LoadAsync(educationId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (education is null)
    {
      await _permissionService.CheckAsync(Actions.CreateEducation, cancellationToken);

      education = new Education(worldId, name, userId, educationId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, education, cancellationToken);

      education.Name = name;
    }

    education.Summary = Summary.TryCreate(payload.Summary);
    education.Description = Description.TryCreate(payload.Description);

    education.Skill = payload.Skill;
    education.WealthMultiplier = WealthMultiplier.TryCreate(payload.WealthMultiplier);
    education.Feature = payload.Feature is null ? null : Feature.Create(payload.Feature.Name, payload.Feature.Description);

    education.Update(userId);

    await _storageService.ExecuteWithQuotaAsync(
      education,
      async () => await _educationRepository.SaveAsync(education, cancellationToken),
      cancellationToken);

    EducationModel model = await _educationQuerier.ReadAsync(education, cancellationToken);
    return new CreateOrReplaceEducationResult(model, created);
  }
}

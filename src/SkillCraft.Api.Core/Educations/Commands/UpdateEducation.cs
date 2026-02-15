using FluentValidation;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Educations;
using SkillCraft.Api.Core.Educations.Validators;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Core.Educations.Commands;

internal record UpdateEducationCommand(Guid Id, UpdateEducationPayload Payload) : ICommand<EducationModel?>;

internal class UpdateEducationCommandHandler : ICommandHandler<UpdateEducationCommand, EducationModel?>
{
  private readonly IContext _context;
  private readonly IEducationQuerier _educationQuerier;
  private readonly IEducationRepository _educationRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public UpdateEducationCommandHandler(
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

  public async Task<EducationModel?> HandleAsync(UpdateEducationCommand command, CancellationToken cancellationToken)
  {
    UpdateEducationPayload payload = command.Payload;
    new UpdateEducationValidator().ValidateAndThrow(payload);

    EducationId educationId = new(command.Id, _context.WorldId);
    Education? education = await _educationRepository.LoadAsync(educationId, cancellationToken);
    if (education is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, education, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      education.Name = new Name(payload.Name);
    }
    if (payload.Summary is not null)
    {
      education.Summary = Summary.TryCreate(payload.Summary.Value);
    }
    if (payload.Description is not null)
    {
      education.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Skill is not null)
    {
      education.Skill = payload.Skill.Value;
    }
    if (payload.WealthMultiplier is not null)
    {
      education.WealthMultiplier = WealthMultiplier.TryCreate(payload.WealthMultiplier.Value);
    }
    if (payload.Feature is not null)
    {
      education.Feature = payload.Feature.Value is null ? null : new Feature(new Name(payload.Feature.Value.Name), Description.TryCreate(payload.Feature.Value.Description));
    }

    education.Update(_context.UserId);

    await _storageService.ExecuteWithQuotaAsync(
      education,
      async () => await _educationRepository.SaveAsync(education, cancellationToken),
      cancellationToken);

    return await _educationQuerier.ReadAsync(education, cancellationToken);
  }
}

using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Educations.Models;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Educations.Commands;

internal record CreateOrReplaceEducationCommand(CreateOrReplaceEducationPayload Payload, Guid? Id) : ICommand<CreateOrReplaceEducationResult>;

internal class CreateOrReplaceEducationCommandHandler : ICommandHandler<CreateOrReplaceEducationCommand, CreateOrReplaceEducationResult>
{
  private readonly IContext _context;
  private readonly IEducationQuerier _educationQuerier;
  private readonly IEducationRepository _educationRepository;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceEducationCommandHandler(
    IContext context,
    IEducationQuerier educationQuerier,
    IEducationRepository educationRepository,
    IPermissionService permissionService)
  {
    _context = context;
    _educationQuerier = educationQuerier;
    _educationRepository = educationRepository;
    _permissionService = permissionService;
  }

  public async Task<CreateOrReplaceEducationResult> HandleAsync(CreateOrReplaceEducationCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceEducationPayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    Education? education = null;
    EducationId educationId = EducationId.NewId(worldId);
    if (command.Id.HasValue)
    {
      educationId = new EducationId(worldId, command.Id.Value);
      education = await _educationRepository.LoadAsync(educationId, cancellationToken);
    }

    Name name = new(payload.Name);
    WealthMultiplier? wealthMultiplier = WealthMultiplier.TryCreate(payload.WealthMultiplier);
    Feature? feature = payload.Feature is null
      ? null
      : new Feature(new Name(payload.Feature.Name), Content.TryCreate(payload.Feature.Content));

    bool created = false;
    if (education is null)
    {
      await _permissionService.CheckAsync(Actions.CreateEducation, cancellationToken);

      education = new Education(educationId, name, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, education, cancellationToken);

      education.Rename(name, actorId);
    }

    education.Edit(Summary.TryCreate(payload.Summary), Content.TryCreate(payload.Content), actorId);
    education.SetRules(payload.Skill, wealthMultiplier, feature, actorId);

    await _educationRepository.SaveAsync(education, cancellationToken);

    EducationModel model = await _educationQuerier.ReadAsync(education, cancellationToken);
    return new CreateOrReplaceEducationResult(model, created);
  }
}

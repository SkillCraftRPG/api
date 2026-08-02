using Logitar.CQRS;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Educations.Models;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Educations.Commands;

internal record UpdateEducationCommand(Guid Id, UpdateEducationPayload Payload) : ICommand<EducationModel?>;

internal class UpdateEducationCommandHandler : ICommandHandler<UpdateEducationCommand, EducationModel?>
{
  private readonly IContext _context;
  private readonly IEducationQuerier _educationQuerier;
  private readonly IEducationRepository _educationRepository;
  private readonly IPermissionService _permissionService;

  public UpdateEducationCommandHandler(
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

  public async Task<EducationModel?> HandleAsync(UpdateEducationCommand command, CancellationToken cancellationToken)
  {
    UpdateEducationPayload payload = command.Payload;
    payload.Validate();

    ActorId? actorId = _context.ActorId;
    WorldId worldId = _context.WorldId;

    EducationId educationId = new(worldId, command.Id);
    Education? education = await _educationRepository.LoadAsync(educationId, cancellationToken);
    if (education is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, education, cancellationToken);

    Name? name = Name.TryCreate(payload.Name);
    if (name is not null)
    {
      education.Rename(name, actorId);
    }

    if (payload.Summary is not null || payload.Content is not null)
    {
      education.Edit(
        payload.Summary is null ? education.Summary : Summary.TryCreate(payload.Summary.Value),
        payload.Content is null ? education.Content : Content.TryCreate(payload.Content.Value),
        actorId);
    }

    if (payload.Skill is not null || payload.WealthMultiplier is not null || payload.Feature is not null)
    {
      Skill? skill = payload.Skill is null ? education.Skill : payload.Skill.Value;
      WealthMultiplier? wealthMultiplier = payload.WealthMultiplier is null
        ? education.WealthMultiplier
        : WealthMultiplier.TryCreate(payload.WealthMultiplier.Value);
      Feature? feature = payload.Feature is null
        ? education.Feature
        : payload.Feature.Value is null
          ? null
          : new Feature(new Name(payload.Feature.Value.Name), Content.TryCreate(payload.Feature.Value.Content));

      education.SetRules(skill, wealthMultiplier, feature, actorId);
    }

    await _educationRepository.SaveAsync(education, cancellationToken);

    return await _educationQuerier.ReadAsync(education, cancellationToken);
  }
}

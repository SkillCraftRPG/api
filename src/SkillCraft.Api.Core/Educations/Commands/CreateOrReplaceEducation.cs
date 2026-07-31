using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Educations.Events;
using SkillCraft.Api.Core.Educations.Models;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Permissions;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Educations.Commands;

internal record CreateOrReplaceEducationCommand(CreateOrReplaceEducationPayload Payload, Guid? Id) : ICommand<CreateOrReplaceEducationResult>;

internal class CreateOrReplaceEducationCommandHandler : ICommandHandler<CreateOrReplaceEducationCommand, CreateOrReplaceEducationResult>
{
  private readonly IContext _context;
  private readonly IEducationRepository _educationRepository;
  private readonly IPermissionService _permissionService;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceEducationCommandHandler(
    IContext context,
    IEducationRepository educationRepository,
    IPermissionService permissionService,
    IWorldRepository worldRepository)
  {
    _context = context;
    _educationRepository = educationRepository;
    _permissionService = permissionService;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceEducationResult> HandleAsync(CreateOrReplaceEducationCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceEducationPayload payload = command.Payload;
    payload.Validate();

    Education? education = null;
    if (command.Id.HasValue)
    {
      education = await _educationRepository.LoadAsync(command.Id.Value, cancellationToken);
    }

    Guid userId = _context.UserId;
    Guid worldId = _context.WorldId;

    EducationSnapshot? snapshot = null;
    if (education is null)
    {
      World world = await _worldRepository.LoadAsync(worldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world 'Id={worldId}' was not found.");
      await _permissionService.CheckAsync(Actions.CreateEducation, world, cancellationToken);

      education = new Education(world, command.Id, userId);
      _educationRepository.Add(education);
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, education, cancellationToken);

      snapshot = new EducationSnapshot(education);
    }

    education.Name = payload.Name.Trim();
    education.Summary = payload.Summary?.CleanTrim();
    education.HtmlContent = payload.HtmlContent?.CleanTrim();

    education.Skill = payload.Skill;
    education.WealthMultiplier = payload.WealthMultiplier;
    education.SetFeature(payload.Feature is null ? null : new Feature(payload.Feature));

    if (snapshot is not null)
    {
      EducationUpdated? record = snapshot.Compare(education);
      if (record is not null)
      {
        education.Update(userId);
        _educationRepository.Update(education, record);
      }
    }

    await _context.SaveChangesAsync(cancellationToken);

    EducationModel model = await _educationRepository.ReadAsync(education, cancellationToken);
    return new CreateOrReplaceEducationResult(model, Created: snapshot is null);
  }
}

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

    Feature? feature = payload.Feature is null ? null : new(payload.Feature);

    bool created = false;
    if (education is null)
    {
      World world = await _worldRepository.LoadAsync(_context.WorldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world 'Id={_context.WorldId}' was not found.");
      await _permissionService.CheckAsync(Actions.CreateEducation, world, cancellationToken);

      education = new Education(world, payload.Name, command.Id, payload.Summary, payload.HtmlContent, payload.Skill, payload.WealthMultiplier, feature, _context.UserId);
      _educationRepository.Add(education);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, education, cancellationToken);

      EducationUpdated record = education.Update(payload.Name, payload.Summary, payload.HtmlContent, payload.Skill, payload.WealthMultiplier, feature, _context.UserId);
      _educationRepository.Update(education, record);
    }

    await _context.SaveChangesAsync(cancellationToken);

    EducationModel model = await _educationRepository.ReadAsync(education, cancellationToken);
    return new CreateOrReplaceEducationResult(model, created);
  }
}

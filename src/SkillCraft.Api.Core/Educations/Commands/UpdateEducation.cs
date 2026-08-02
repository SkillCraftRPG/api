using Logitar;
using Logitar.CQRS;
using SkillCraft.Api.Core.Educations.Events;
using SkillCraft.Api.Core.Educations.Models;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.Core.Educations.Commands;

internal record UpdateEducationCommand(Guid Id, UpdateEducationPayload Payload) : ICommand<EducationModel?>;

internal class UpdateEducationCommandHandler : ICommandHandler<UpdateEducationCommand, EducationModel?>
{
  private readonly IContext _context;
  private readonly IEducationRepository _educationRepository;
  private readonly IPermissionService _permissionService;

  public UpdateEducationCommandHandler(IContext context, IEducationRepository educationRepository, IPermissionService permissionService)
  {
    _context = context;
    _educationRepository = educationRepository;
    _permissionService = permissionService;
  }

  public async Task<EducationModel?> HandleAsync(UpdateEducationCommand command, CancellationToken cancellationToken)
  {
    UpdateEducationPayload payload = command.Payload;
    payload.Validate();

    Education? education = await _educationRepository.LoadAsync(command.Id, cancellationToken);
    if (education is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, education, cancellationToken);

    EducationSnapshot snapshot = new(education);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      education.Name = payload.Name.Trim();
    }
    if (payload.Summary is not null)
    {
      education.Summary = payload.Summary.Value?.CleanTrim();
    }
    if (payload.Content is not null)
    {
      education.Content = payload.Content.Value?.CleanTrim();
    }
    if (payload.Skill is not null)
    {
      education.Skill = payload.Skill.Value;
    }
    if (payload.WealthMultiplier is not null)
    {
      education.WealthMultiplier = payload.WealthMultiplier.Value;
    }
    if (payload.Feature is not null)
    {
      education.SetFeature(payload.Feature.Value is null ? null : new FeatureOld(payload.Feature.Value));
    }

    EducationUpdated? record = snapshot.Compare(education);
    if (record is not null)
    {
      education.Update(_context.UserUid);
      _educationRepository.Update(education, record);

      await _context.SaveChangesAsync(cancellationToken);
    }

    return await _educationRepository.ReadAsync(education, cancellationToken);
  }
}

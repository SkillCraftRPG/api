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

    Feature? feature = null;
    if (payload.Feature is null)
    {
      if (education.FeatureName is not null)
      {
        feature = new Feature(education.FeatureName, education.FeatureHtmlContent);
      }
    }
    else if (payload.Feature.Value is not null)
    {
      feature = new Feature(payload.Feature.Value);
    }

    EducationUpdated record = education.Update(
      string.IsNullOrWhiteSpace(payload.Name) ? education.Name : payload.Name,
      payload.Summary is null ? education.Summary : payload.Summary.Value,
      payload.HtmlContent is null ? education.HtmlContent : payload.HtmlContent.Value,
      payload.Skill is null ? education.Skill : payload.Skill.Value,
      payload.WealthMultiplier is null ? education.WealthMultiplier : payload.WealthMultiplier.Value,
      feature,
      _context.UserId);
    _educationRepository.Update(education, record);

    await _context.SaveChangesAsync(cancellationToken);

    return await _educationRepository.ReadAsync(education, cancellationToken);
  }
}

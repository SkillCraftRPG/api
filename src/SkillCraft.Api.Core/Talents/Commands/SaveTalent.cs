using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents.Commands;

internal abstract class SaveTalent
{
  protected virtual ITalentRepository TalentRepository { get; }

  protected SaveTalent(ITalentRepository talentRepository)
  {
    TalentRepository = talentRepository;
  }

  protected virtual async Task SetRequiredTalentAsync(Talent talent, Guid? requiredTalentId, WorldId worldId, CancellationToken cancellationToken)
  {
    Talent? requiredTalent = null;
    if (requiredTalentId.HasValue)
    {
      TalentId talentId = new(requiredTalentId.Value, worldId);
      requiredTalent = await TalentRepository.LoadAsync(talentId, cancellationToken)
        ?? throw new EntityNotFoundException(new Entity(Talent.EntityKind, requiredTalentId.Value, worldId), nameof(Talent.RequiredTalentId));
    }
    talent.SetRequiredTalent(requiredTalent);
  }
}

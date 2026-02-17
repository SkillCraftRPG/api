using SkillCraft.Api.Contracts.Specializations;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Specializations.Commands;

internal abstract class SaveSpecialization
{
  protected virtual ITalentRepository TalentRepository { get; }

  protected SaveSpecialization(ITalentRepository talentRepository)
  {
    TalentRepository = talentRepository;
  }

  protected virtual async Task<IReadOnlyDictionary<Guid, Talent>> LoadTalentsAsync(
    RequirementsPayload? requirements,
    WorldId worldId,
    CancellationToken cancellationToken)
  {
    HashSet<TalentId> talentIds = new(capacity: 1); // TODO(fpion): capacity
    if (requirements is not null && requirements.TalentId.HasValue)
    {
      talentIds.Add(new TalentId(requirements.TalentId.Value, worldId));
    }
    return (await TalentRepository.LoadAsync(talentIds, cancellationToken)).ToDictionary(x => x.EntityId, x => x);
  }

  protected virtual void SetRequirements(Specialization specialization, RequirementsPayload requirements, IReadOnlyDictionary<Guid, Talent> talents)
  {
    Talent? requiredTalent = null;
    if (requirements.TalentId.HasValue && !talents.TryGetValue(requirements.TalentId.Value, out requiredTalent))
    {
      throw new EntityNotFoundException(new Entity(Talent.EntityKind, requirements.TalentId.Value, specialization.WorldId), "Requirements.TalentId"); // TODO(fpion): no magic strings
    }
    specialization.SetRequirements(requiredTalent, requirements.Other);
  }
}

using Logitar;
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
    OptionsPayload? options,
    WorldId worldId,
    CancellationToken cancellationToken)
  {
    HashSet<TalentId> talentIds = new(capacity: 1 + (options?.TalentIds.Count ?? 0)); // TODO(fpion): Doctrine
    if (requirements is not null && requirements.TalentId.HasValue)
    {
      talentIds.Add(new TalentId(requirements.TalentId.Value, worldId));
    }
    if (options is not null)
    {
      talentIds.AddRange(options.TalentIds.Select(id => new TalentId(id, worldId)));
    }
    return talentIds.Count < 1 ? [] : (await TalentRepository.LoadAsync(talentIds, cancellationToken)).ToDictionary(x => x.EntityId, x => x);
  }

  protected virtual void SetRequirements(Specialization specialization, RequirementsPayload requirements, IReadOnlyDictionary<Guid, Talent> talents)
  {
    Talent? requiredTalent = null;
    if (requirements.TalentId.HasValue && !talents.TryGetValue(requirements.TalentId.Value, out requiredTalent))
    {
      string propertyName = string.Join('.', nameof(Specialization.Requirements), nameof(requirements.TalentId));
      throw new EntityNotFoundException(new Entity(Talent.EntityKind, requirements.TalentId.Value, specialization.WorldId), propertyName);
    }
    specialization.SetRequirements(requiredTalent, requirements.Other);
  }

  protected virtual void SetOptions(Specialization specialization, OptionsPayload options, IReadOnlyDictionary<Guid, Talent> talents)
  {
    int capacity = options.TalentIds.Count;
    List<Talent> optionalTalents = new(capacity);
    HashSet<Guid> missingIds = new(capacity);
    foreach (Guid talentId in options.TalentIds)
    {
      if (talents.TryGetValue(talentId, out Talent? talent))
      {
        optionalTalents.Add(talent);
      }
      else
      {
        missingIds.Add(talentId);
      }
    }
    if (missingIds.Count > 0)
    {
      string propertyName = string.Join('.', nameof(Specialization.Options), nameof(options.TalentIds));
      throw new TalentsNotFoundException(specialization.WorldId, missingIds, propertyName);
    }
    specialization.SetOptions(optionalTalents, options.Other);
  }
}

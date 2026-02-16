using Logitar.EventSourcing;
using SkillCraft.Api.Contracts;

namespace SkillCraft.Api.Core.Talents.Events;

public record TalentUpdated : DomainEvent
{
  public Name? Name { get; set; }
  public Change<Summary>? Summary { get; set; }
  public Change<Description>? Description { get; set; }

  public bool? AllowMultiplePurchases { get; set; }
  public Change<GameSkill?>? Skill { get; set; }
  public Change<TalentId?>? RequiredTalentId { get; set; }
}

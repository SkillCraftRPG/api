using Logitar.EventSourcing;
using SkillCraft.Api.Contracts;

namespace SkillCraft.Api.Core.Castes.Events;

public record CasteUpdated : DomainEvent
{
  public Name? Name { get; set; }
  public Change<Summary>? Summary { get; set; }
  public Change<Description>? Description { get; set; }

  public Change<GameSkill?>? Skill { get; set; }
  public Change<Roll>? WealthRoll { get; set; }
  public Change<Feature>? Feature { get; set; }
}

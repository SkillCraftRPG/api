using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterExperienceGained(int Experience) : DomainEvent;

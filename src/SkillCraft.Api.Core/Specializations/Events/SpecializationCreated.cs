using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Specializations.Events;

public record SpecializationCreated(Tier Tier, Name Name) : DomainEvent;

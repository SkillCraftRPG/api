using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Specializations.Events;

public record SpecializationDeleted : DomainEvent, IDeleteEvent;

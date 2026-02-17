using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Lineages.Events;

public record LineageDeleted : DomainEvent, IDeleteEvent;

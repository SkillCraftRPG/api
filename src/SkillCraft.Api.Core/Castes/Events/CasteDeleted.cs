using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Castes.Events;

public record CasteDeleted : DomainEvent, IDeleteEvent;

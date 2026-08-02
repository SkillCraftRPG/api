using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Items.Events;

public record ItemDeleted : DomainEvent, IDeleteEvent;

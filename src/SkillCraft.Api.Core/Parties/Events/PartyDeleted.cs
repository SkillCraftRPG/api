using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Parties.Events;

public record PartyDeleted : DomainEvent, IDeleteEvent;

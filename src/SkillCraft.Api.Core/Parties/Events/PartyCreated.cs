using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Parties.Events;

public record PartyCreated(Name Name) : DomainEvent;

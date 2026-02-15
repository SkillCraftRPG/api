using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Castes.Events;

public record CasteCreated(Name Name) : DomainEvent;

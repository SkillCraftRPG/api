using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Castes.Events;

public record CasteRenamed(Name Name) : DomainEvent;

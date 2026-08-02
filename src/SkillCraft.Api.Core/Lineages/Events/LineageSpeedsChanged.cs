using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Lineages.Events;

public record LineageSpeedsChanged(LineageSpeeds Speeds) : DomainEvent;

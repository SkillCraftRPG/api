using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Scripts.Events;

public record ScriptCreated(Name Name) : DomainEvent;

using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Scripts.Events;

public record ScriptRenamed(Name Name) : DomainEvent;

using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Scripts.Events;

public record ScriptEdited(Summary? Summary, Content? Content) : DomainEvent;

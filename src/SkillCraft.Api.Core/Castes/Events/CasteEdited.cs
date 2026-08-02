using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Castes.Events;

public record CasteEdited(Summary? Summary, Content? Content) : DomainEvent;

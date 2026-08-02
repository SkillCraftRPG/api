using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Customizations.Events;

public record CustomizationEdited(Summary? Summary, Content? Content) : DomainEvent;

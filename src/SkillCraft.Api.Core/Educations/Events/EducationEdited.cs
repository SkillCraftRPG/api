using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Educations.Events;

public record EducationEdited(Summary? Summary, Content? Content) : DomainEvent;

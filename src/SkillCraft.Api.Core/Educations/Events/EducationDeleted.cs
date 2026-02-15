using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Educations.Events;

public record EducationDeleted : DomainEvent, IDeleteEvent;

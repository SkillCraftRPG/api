using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Educations.Events;

public record EducationCreated(Name Name) : DomainEvent;

using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Educations.Events;

public record EducationRenamed(Name Name) : DomainEvent;

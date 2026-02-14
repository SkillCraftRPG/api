using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Storages.Events;

public record StorageDeleted : DomainEvent, IDeleteEvent;

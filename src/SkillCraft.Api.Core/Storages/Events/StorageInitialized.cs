using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Storages.Events;

public record StorageInitialized(long AllocatedBytes) : DomainEvent;

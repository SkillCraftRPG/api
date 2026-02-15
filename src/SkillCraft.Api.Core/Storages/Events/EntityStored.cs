using Logitar.EventSourcing;

namespace SkillCraft.Api.Core.Storages.Events;

public record EntityStored(string Key, long Size) : DomainEvent;

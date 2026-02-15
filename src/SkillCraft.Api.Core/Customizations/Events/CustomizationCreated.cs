using Logitar.EventSourcing;
using SkillCraft.Api.Contracts.Customizations;

namespace SkillCraft.Api.Core.Customizations.Events;

public record CustomizationCreated(CustomizationKind Kind, Name Name) : DomainEvent;

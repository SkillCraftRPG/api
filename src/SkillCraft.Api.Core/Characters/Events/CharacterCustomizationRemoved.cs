using Logitar.EventSourcing;
using SkillCraft.Api.Core.Customizations;

namespace SkillCraft.Api.Core.Characters.Events;

public record CharacterCustomizationRemoved(CustomizationId CustomizationId) : DomainEvent;

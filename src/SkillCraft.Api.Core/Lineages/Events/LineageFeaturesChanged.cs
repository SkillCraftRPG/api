using Logitar.EventSourcing;
using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Core.Lineages.Events;

public record LineageFeaturesChanged(IReadOnlyCollection<Feature> Features) : DomainEvent;

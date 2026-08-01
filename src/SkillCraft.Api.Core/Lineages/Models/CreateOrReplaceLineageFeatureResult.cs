namespace SkillCraft.Api.Core.Lineages.Models;

public record CreateOrReplaceLineageFeatureResult(LineageModel Lineage, Guid FeatureId, bool Created);

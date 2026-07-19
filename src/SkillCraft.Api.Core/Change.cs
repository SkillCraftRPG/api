namespace SkillCraft.Api.Core;

public record Change<T>(T? OldValue, T? NewValue);

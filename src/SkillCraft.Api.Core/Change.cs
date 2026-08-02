namespace SkillCraft.Api.Core;

public record Change<T>(T? OldValue, T? NewValue); // TODO(fpion): remove this

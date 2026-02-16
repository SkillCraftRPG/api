using SkillCraft.Api.Core.Scripts;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class ScriptIdConverter : JsonConverter<ScriptId>
{
  public override ScriptId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new ScriptId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, ScriptId scriptId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(scriptId.Value);
  }
}

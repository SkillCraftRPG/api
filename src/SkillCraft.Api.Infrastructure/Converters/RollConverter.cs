using SkillCraft.Api.Core;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class RollConverter : JsonConverter<Roll>
{
  public override Roll? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Roll.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Roll roll, JsonSerializerOptions options)
  {
    writer.WriteStringValue(roll.Value);
  }
}

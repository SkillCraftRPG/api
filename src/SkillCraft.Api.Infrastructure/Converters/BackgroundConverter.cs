using SkillCraft.Api.Core.Characters;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class BackgroundConverter : JsonConverter<Background>
{
  public override Background? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Background.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Background background, JsonSerializerOptions options)
  {
    writer.WriteStringValue(background.Value);
  }
}

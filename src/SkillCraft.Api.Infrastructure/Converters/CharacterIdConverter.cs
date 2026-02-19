using SkillCraft.Api.Core.Characters;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class CharacterIdConverter : JsonConverter<CharacterId>
{
  public override CharacterId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new CharacterId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, CharacterId characterId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(characterId.Value);
  }
}

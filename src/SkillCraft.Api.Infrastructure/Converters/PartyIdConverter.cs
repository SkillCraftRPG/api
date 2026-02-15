using SkillCraft.Api.Core.Parties;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class PartyIdConverter : JsonConverter<PartyId>
{
  public override PartyId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new PartyId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, PartyId partyId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(partyId.Value);
  }
}

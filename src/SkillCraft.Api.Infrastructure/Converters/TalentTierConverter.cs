using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Infrastructure.Converters;

internal class TalentTierConverter : JsonConverter<TalentTier>
{
  public override TalentTier? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetInt32(out int value) ? new TalentTier(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, TalentTier tier, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(tier.Value);
  }
}

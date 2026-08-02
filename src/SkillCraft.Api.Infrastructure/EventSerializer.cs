using SkillCraft.Api.Infrastructure.Converters;

namespace SkillCraft.Api.Infrastructure;

internal class EventSerializer : Logitar.EventSourcing.Infrastructure.EventSerializer
{
  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new CasteIdConverter());
    SerializerOptions.Converters.Add(new ContentConverter());
    SerializerOptions.Converters.Add(new CustomizationIdConverter());
    SerializerOptions.Converters.Add(new EducationIdConverter());
    SerializerOptions.Converters.Add(new KeyConverter());
    SerializerOptions.Converters.Add(new NameConverter());
    SerializerOptions.Converters.Add(new RollConverter());
    SerializerOptions.Converters.Add(new ScriptIdConverter());
    SerializerOptions.Converters.Add(new SummaryConverter());
    SerializerOptions.Converters.Add(new TalentIdConverter());
    SerializerOptions.Converters.Add(new TalentTierConverter());
    SerializerOptions.Converters.Add(new UserIdConverter());
    SerializerOptions.Converters.Add(new WealthMultiplierConverter());
    SerializerOptions.Converters.Add(new WorldIdConverter());
  }
}

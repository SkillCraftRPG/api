using SkillCraft.Api.Infrastructure.Converters;

namespace SkillCraft.Api.Infrastructure;

internal class EventSerializer : Logitar.EventSourcing.Infrastructure.EventSerializer
{
  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new CasteIdConverter());
    SerializerOptions.Converters.Add(new CharacterIdConverter());
    SerializerOptions.Converters.Add(new CustomizationIdConverter());
    SerializerOptions.Converters.Add(new DescriptionConverter());
    SerializerOptions.Converters.Add(new EducationIdConverter());
    SerializerOptions.Converters.Add(new LanguageIdConverter());
    SerializerOptions.Converters.Add(new LineageIdConverter());
    SerializerOptions.Converters.Add(new NameConverter());
    SerializerOptions.Converters.Add(new PartyIdConverter());
    SerializerOptions.Converters.Add(new RollConverter());
    SerializerOptions.Converters.Add(new ScriptIdConverter());
    SerializerOptions.Converters.Add(new SpecializationIdConverter());
    SerializerOptions.Converters.Add(new StorageIdConverter());
    SerializerOptions.Converters.Add(new SummaryConverter());
    SerializerOptions.Converters.Add(new TalentIdConverter());
    SerializerOptions.Converters.Add(new TierConverter());
    SerializerOptions.Converters.Add(new TypicalSpeakersConverter());
    SerializerOptions.Converters.Add(new UserIdConverter());
    SerializerOptions.Converters.Add(new WealthMultiplierConverter());
    SerializerOptions.Converters.Add(new WorldIdConverter());
  }
}

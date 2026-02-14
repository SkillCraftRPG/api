using SkillCraft.Api.Infrastructure.Converters;

namespace SkillCraft.Api.Infrastructure;

internal class EventSerializer : Logitar.EventSourcing.Infrastructure.EventSerializer
{
  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new CustomizationIdConverter());
    SerializerOptions.Converters.Add(new DescriptionConverter());
    SerializerOptions.Converters.Add(new NameConverter());
    SerializerOptions.Converters.Add(new StorageIdConverter());
    SerializerOptions.Converters.Add(new SummaryConverter());
    SerializerOptions.Converters.Add(new UserIdConverter());
    SerializerOptions.Converters.Add(new WorldIdConverter());
  }
}

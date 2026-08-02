using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Lineages.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class LineageEntity : AggregateEntity
{
  public int LineageId { get; private set; }

  public WorldEntity? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public LineageEntity? Parent { get; private set; }
  public List<LineageEntity> Children { get; private set; } = [];
  public int? ParentId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Content { get; private set; }

  public int ExtraLanguages { get; private set; }
  public string? LanguagesContent { get; private set; }

  public string? FamilyNames { get; private set; }
  public string? FemaleNames { get; private set; }
  public string? MaleNames { get; private set; }
  public string? UnisexNames { get; private set; }
  public string? CustomNames { get; private set; }
  public string? NamesContent { get; private set; }

  public int? Walk { get; private set; }
  public int? Climb { get; private set; }
  public int? Swim { get; private set; }
  public int? Fly { get; private set; }
  public bool Hover { get; private set; }
  public int? Burrow { get; private set; }

  public SizeCategory SizeCategory { get; private set; }
  public string? HeightRoll { get; private set; }

  public string? Malnutrition { get; private set; }
  public string? Skinny { get; private set; }
  public string? NormalWeight { get; private set; }
  public string? Overweight { get; private set; }
  public string? Obese { get; private set; }

  public int? Teenager { get; private set; }
  public int? Adult { get; private set; }
  public int? Mature { get; private set; }
  public int? Venerable { get; private set; }

  public List<LineageFeatureEntity> Features { get; private set; } = [];
  public List<LanguageEntity> Languages { get; private set; } = [];

  public LineageEntity(Lineage lineage, int? parentId, IEnumerable<LanguageEntity> languages) : base(lineage)
  {
    WorldId = lineage.WorldId.ResourceId;
    Id = lineage.ResourceId;
    ParentId = parentId;

    Name = lineage.Name.Value;

    Update(lineage, parentId, languages);
  }

  public LineageEntity(LineageCreated @event, int? parentId) : base(@event)
  {
    LineageId lineageId = new(@event.StreamId);
    WorldId = lineageId.WorldId.ResourceId;
    Id = lineageId.ResourceId;
    ParentId = parentId;

    Name = @event.Name.Value;
  }

  private LineageEntity() : base()
  {
  }

  public void Edit(LineageEdited @event)
  {
    base.Update(@event);

    Summary = @event.Summary?.Value;
    Content = @event.Content?.Value;
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (Parent is not null)
    {
      actorIds.AddRange(Parent.GetActorIds());
    }
    foreach (LineageFeatureEntity feature in Features)
    {
      actorIds.AddRange(feature.GetActorIds());
    }
    foreach (LanguageEntity language in Languages)
    {
      actorIds.AddRange(language.GetActorIds());
    }
    return actorIds.AsReadOnly();
  }

  public void Rename(LineageRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name.Value;
  }

  public void SetLanguages(IEnumerable<LanguageEntity> languages, LineageLanguagesChanged @event)
  {
    base.Update(@event);

    ExtraLanguages = @event.Languages.Extra;
    LanguagesContent = @event.Languages.Content?.Value;
    Languages.Clear();
    Languages.AddRange(languages);
  }

  public void SetNames(LineageNamesChanged @event)
  {
    base.Update(@event);

    SetNames(@event.Names);
  }

  public void SetSpeeds(LineageSpeedsChanged @event)
  {
    base.Update(@event);

    SetSpeeds(@event.Speeds);
  }

  public void SetTraits(LineageTraitsChanged @event)
  {
    base.Update(@event);

    SetTraits(@event.Size, @event.Weight, @event.Age);
  }

  public void Update(Lineage lineage, int? parentId, IEnumerable<LanguageEntity> languages)
  {
    base.Update(lineage);

    ParentId = parentId;

    Name = lineage.Name.Value;
    Summary = lineage.Summary?.Value;
    Content = lineage.Content?.Value;

    ExtraLanguages = lineage.Languages.Extra;
    LanguagesContent = lineage.Languages.Content?.Value;
    Languages.Clear();
    Languages.AddRange(languages);

    SetNames(lineage.Names);
    SetSpeeds(lineage.Speeds);
    SetTraits(lineage.Size, lineage.Weight, lineage.Age);
  }

  private void SetNames(LineageNames names)
  {
    FamilyNames = EncodeNames(names.Family);
    FemaleNames = EncodeNames(names.Female);
    MaleNames = EncodeNames(names.Male);
    UnisexNames = EncodeNames(names.Unisex);
    CustomNames = names.Custom.Count < 1 ? null : JsonSerializer.Serialize(names.Custom);
    NamesContent = names.Content?.Value;
  }

  private void SetSpeeds(LineageSpeeds speeds)
  {
    Walk = speeds.Walk;
    Climb = speeds.Climb;
    Swim = speeds.Swim;
    Fly = speeds.Fly;
    Hover = speeds.Hover;
    Burrow = speeds.Burrow;
  }

  private void SetTraits(LineageSize size, LineageWeight weight, LineageAge age)
  {
    SizeCategory = size.Category;
    HeightRoll = size.Height?.Value;

    Malnutrition = weight.Malnutrition?.Value;
    Skinny = weight.Skinny?.Value;
    NormalWeight = weight.Normal?.Value;
    Overweight = weight.Overweight?.Value;
    Obese = weight.Obese?.Value;

    Teenager = age.Teenager;
    Adult = age.Adult;
    Mature = age.Mature;
    Venerable = age.Venerable;
  }

  public static IReadOnlyCollection<string> DecodeNames(string? names) =>
    (names is null ? null : JsonSerializer.Deserialize<IReadOnlyCollection<string>>(names)) ?? [];

  public static IReadOnlyDictionary<string, IReadOnlyCollection<string>> DecodeCustomNames(string? custom) =>
    (custom is null ? null : JsonSerializer.Deserialize<IReadOnlyDictionary<string, IReadOnlyCollection<string>>>(custom))
    ?? new Dictionary<string, IReadOnlyCollection<string>>().AsReadOnly();

  private static string? EncodeNames(IEnumerable<string> names) => names.Any() ? JsonSerializer.Serialize(names) : null;

  public override string ToString() => $"{Name} | {base.ToString()}";
}

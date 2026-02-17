using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Lineages.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class LineageEntity : AggregateEntity, IWorldScoped
{
  public int LineageId { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }
  public Guid WorldUid { get; private set; }

  public Guid Id { get; private set; }

  public LineageEntity? Parent { get; private set; }
  public int? ParentId { get; private set; }
  public Guid? ParentUid { get; private set; }
  public List<LineageEntity> Children { get; private set; } = [];

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Description { get; private set; }

  public string? Features { get; private set; }

  public List<LineageLanguageEntity> Languages { get; private set; } = [];
  public int ExtraLanguages { get; private set; }
  public string? LanguagesText { get; private set; }

  public string? FamilyNames { get; private set; }
  public string? FemaleNames { get; private set; }
  public string? MaleNames { get; private set; }
  public string? UnisexNames { get; private set; }
  public string? CustomNames { get; private set; }
  public string? NamesText { get; private set; }

  public int Walk { get; private set; }
  public int Climb { get; private set; }
  public int Swim { get; private set; }
  public int Fly { get; private set; }
  public bool Hover { get; private set; }
  public int Burrow { get; private set; }

  public SizeCategory SizeCategory { get; private set; }
  public string? Height { get; private set; }

  public string? Malnutrition { get; private set; }
  public string? Skinny { get; private set; }
  public string? Normal { get; private set; }
  public string? Overweight { get; private set; }
  public string? Obese { get; private set; }

  public int Teenager { get; private set; }
  public int Adult { get; private set; }
  public int Mature { get; private set; }
  public int Venerable { get; private set; }

  public LineageEntity(WorldEntity world, LineageEntity? parent, LineageCreated @event) : base(@event)
  {
    Id = new LineageId(@event.StreamId).EntityId;

    World = world;
    WorldId = world.WorldId;
    WorldUid = world.Id;

    Parent = parent;
    ParentId = parent?.LineageId;
    ParentUid = parent?.Id;

    Name = @event.Name.Value;
  }

  private LineageEntity() : base()
  {
  }

  public void AddLanguage(LanguageEntity language)
  {
    Languages.Add(new LineageLanguageEntity(this, language));
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (Parent is not null)
    {
      actorIds.AddRange(Parent.GetActorIds());
    }
    foreach (LineageLanguageEntity entity in Languages)
    {
      if (entity.Language is not null)
      {
        actorIds.AddRange(entity.Language.GetActorIds());
      }
    }
    return actorIds;
  }

  public void Update(LineageUpdated @event)
  {
    base.Update(@event);

    if (@event.Name is not null)
    {
      Name = @event.Name.Value;
    }
    if (@event.Summary is not null)
    {
      Summary = @event.Summary.Value?.Value;
    }
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Features is not null)
    {
      SetFeatures(@event.Features);
    }
    if (@event.Languages is not null)
    {
      ExtraLanguages = @event.Languages.Extra;
      LanguagesText = @event.Languages.Text?.Value;
    }
    if (@event.Names is not null)
    {
      SetNames(@event.Names);
    }

    if (@event.Speeds is not null)
    {
      SetSpeeds(@event.Speeds);
    }
    if (@event.Size is not null)
    {
      SetSize(@event.Size);
    }
    if (@event.Weight is not null)
    {
      SetWeight(@event.Weight);
    }
    if (@event.Age is not null)
    {
      SetAge(@event.Age);
    }
  }

  public IReadOnlyCollection<FeatureModel> GetFeatures()
  {
    Dictionary<string, string?> features = (Features is null ? null : JsonSerializer.Deserialize<Dictionary<string, string?>>(Features)) ?? [];
    return features.Select(feature => new FeatureModel(feature.Key, feature.Value)).ToList().AsReadOnly();
  }
  private void SetFeatures(IEnumerable<Feature> features)
  {
    Features = features.Any() ? JsonSerializer.Serialize(features.GroupBy(x => x.Name.Value).ToDictionary(x => x.Key, x => x.Last().Description?.Value)) : null;
  }

  public NamesModel GetNames()
  {
    NamesModel names = new();
    if (FamilyNames is not null)
    {
      names.Family.AddRange(JsonSerializer.Deserialize<string[]>(FamilyNames) ?? []);
    }
    if (FemaleNames is not null)
    {
      names.Female.AddRange(JsonSerializer.Deserialize<string[]>(FemaleNames) ?? []);
    }
    if (MaleNames is not null)
    {
      names.Male.AddRange(JsonSerializer.Deserialize<string[]>(MaleNames) ?? []);
    }
    if (UnisexNames is not null)
    {
      names.Unisex.AddRange(JsonSerializer.Deserialize<string[]>(UnisexNames) ?? []);
    }
    if (CustomNames is not null)
    {
      Dictionary<string, string[]> custom = JsonSerializer.Deserialize<Dictionary<string, string[]>>(CustomNames) ?? [];
      foreach (KeyValuePair<string, string[]> nameCategory in custom)
      {
        names.Custom.Add(new NameCategory(nameCategory.Key, nameCategory.Value));
      }
    }
    names.Text = NamesText;
    return names;
  }
  private void SetNames(Names names)
  {
    FamilyNames = names.Family.Count < 1 ? null : JsonSerializer.Serialize(names.Family);
    FemaleNames = names.Female.Count < 1 ? null : JsonSerializer.Serialize(names.Female);
    MaleNames = names.Male.Count < 1 ? null : JsonSerializer.Serialize(names.Male);
    UnisexNames = names.Unisex.Count < 1 ? null : JsonSerializer.Serialize(names.Unisex);
    CustomNames = names.Custom.Count < 1 ? null : JsonSerializer.Serialize(names.Custom);
    NamesText = names.Text?.Value;
  }

  public SpeedsModel GetSpeeds() => new(Walk, Climb, Swim, Fly, Hover, Burrow);
  private void SetSpeeds(Speeds speeds)
  {
    Walk = speeds.Walk;
    Climb = speeds.Climb;
    Swim = speeds.Swim;
    Fly = speeds.Fly;
    Hover = speeds.Hover;
    Burrow = speeds.Burrow;
  }

  public SizeModel GetSize() => new(SizeCategory, Height);
  private void SetSize(Size size)
  {
    SizeCategory = size.Category;
    Height = size.Height?.Value;
  }

  public WeightModel GetWeight() => new(MaleNames, Skinny, Normal, Overweight, Obese);
  private void SetWeight(Weight weight)
  {
    Malnutrition = weight.Malnutrition?.Value;
    Skinny = weight.Skinny?.Value;
    Normal = weight.Normal?.Value;
    Overweight = weight.Overweight?.Value;
    Obese = weight.Obese?.Value;
  }

  public AgeModel GetAge() => new(Teenager, Adult, Mature, Venerable);
  private void SetAge(Age age)
  {
    Teenager = age.Teenager;
    Adult = age.Adult;
    Mature = age.Mature;
    Venerable = age.Venerable;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}

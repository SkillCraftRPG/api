using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Core.Permissions;

namespace SkillCraft.Api.IntegrationTests.Lineages;

[Trait(Traits.Category, Categories.Integration)]
public class LineageFeatureIntegrationTests : IntegrationTests
{
  private readonly ILineageRepository _lineageRepository;
  private readonly ILineageService _lineageService;

  private LineageFeature _feature = null!;
  private Lineage _lineage = null!;

  public LineageFeatureIntegrationTests() : base()
  {
    _lineageRepository = ServiceProvider.GetRequiredService<ILineageRepository>();
    _lineageService = ServiceProvider.GetRequiredService<ILineageService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _lineage = LineageBuilder.Elfe(Faker, Context.World);
    _lineageRepository.Add(_lineage);

    _feature = new LineageFeature(_lineage, Context.UserUid)
    {
      Name = "Transe",
      Content = "Le personnage peut remplacer la nuit de sommeil conventionnelle de 8 heures par une transe d’une durée de seulement 4 heures. Compléter cette transe procure les mêmes effets que de compléter une nuit de sommeil. Pendant cette transe, le personnage est en état de conscience partielle, mélangeant rêves éveillés et lucidité détachée. Il demeure partiellement réceptif à son environnement et peut effectuer avec désavantage des tests passifs."
    };
    _lineage.Features.Add(_feature);
    _lineageRepository.Add(_feature);

    await Context.SaveChangesAsync();
  }

  [Theory(DisplayName = "It should create a new lineage feature.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    FeatureModel payload = CreateEspritEveillePayload();
    Guid? featureId = withId ? Guid.NewGuid() : null;

    CreateOrReplaceLineageFeatureResult result = await _lineageService.CreateOrReplaceFeatureAsync(_lineage.Id, payload, featureId);
    Assert.True(result.Created);
    LineageModel lineage = result.Lineage;
    Assert.NotNull(lineage);

    Assert.Equal(2, lineage.Version);
    Assert.Equal(_lineage.CreatedBy, lineage.CreatedBy.Id);
    Assert.Equal(_lineage.CreatedOn, lineage.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, lineage.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, lineage.UpdatedOn, TimeSpan.FromSeconds(10));

    if (featureId.HasValue)
    {
      Assert.Equal(featureId.Value, result.FeatureId);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, result.FeatureId);
    }

    LineageFeatureModel feature = Assert.Single(lineage.Features, x => x.Id == result.FeatureId);
    AssertEspritEveille(payload, feature);
    Assert.Equal(Actor, feature.CreatedBy);
    Assert.Equal(DateTime.UtcNow, feature.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(feature.CreatedBy, feature.UpdatedBy);
    Assert.Equal(feature.CreatedOn, feature.UpdatedOn);
  }

  [Fact(DisplayName = "It should delete an existing lineage feature.")]
  public async Task Given_Exists_When_Delete_Then_Deleted()
  {
    LineageModel? lineage = await _lineageService.DeleteFeatureAsync(_lineage.Id, _feature.Id);
    Assert.NotNull(lineage);

    Assert.Equal(_lineage.Id, lineage.Id);
    Assert.Equal(2, lineage.Version);
    Assert.Equal(_lineage.CreatedBy, lineage.CreatedBy.Id);
    Assert.Equal(_lineage.CreatedOn, lineage.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, lineage.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, lineage.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.DoesNotContain(lineage.Features, feature => feature.Id == _feature.Id);
  }

  [Fact(DisplayName = "It should replace an existing lineage feature.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    FeatureModel payload = CreateSensAffutesPayload();
    Guid featureId = _feature.Id;

    CreateOrReplaceLineageFeatureResult result = await _lineageService.CreateOrReplaceFeatureAsync(_lineage.Id, payload, featureId);
    Assert.False(result.Created);
    Assert.Equal(featureId, result.FeatureId);

    LineageModel lineage = result.Lineage;
    Assert.NotNull(lineage);

    Assert.Equal(2, lineage.Version);
    Assert.Equal(_lineage.CreatedBy, lineage.CreatedBy.Id);
    Assert.Equal(_lineage.CreatedOn, lineage.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, lineage.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, lineage.UpdatedOn, TimeSpan.FromSeconds(10));

    LineageFeatureModel feature = Assert.Single(lineage.Features, x => x.Id == featureId);
    AssertSensAffutes(payload, feature);
    Assert.Equal(_feature.CreatedBy, feature.CreatedBy.Id);
    Assert.Equal(_feature.CreatedOn, feature.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, feature.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, feature.UpdatedOn, TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "It should return null when the feature was not found.")]
  public async Task Given_FeatureNotFound_When_Delete_Then_NullReturned()
  {
    Assert.Null(await _lineageService.DeleteFeatureAsync(_lineage.Id, Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the lineage was not found.")]
  public async Task Given_LineageNotFound_When_Delete_Then_NullReturned()
  {
    Assert.Null(await _lineageService.DeleteFeatureAsync(Guid.Empty, _feature.Id));
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a lineage feature.")]
  public async Task Given_NotAllowed_When_Create_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    FeatureModel payload = CreateEspritEveillePayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _lineageService.CreateOrReplaceFeatureAsync(_lineage.Id, payload));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_lineage.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when deleting a lineage feature.")]
  public async Task Given_NotAllowed_When_Delete_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _lineageService.DeleteFeatureAsync(_lineage.Id, _feature.Id));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_lineage.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing a lineage feature.")]
  public async Task Given_NotAllowed_When_Replace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder(Faker).Build();

    FeatureModel payload = CreateSensAffutesPayload();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _lineageService.CreateOrReplaceFeatureAsync(_lineage.Id, payload, _feature.Id));
    Assert.Equal(Context.UserUid, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(_lineage.Identifier.ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw ResourceNotFoundException when the lineage was not found.")]
  public async Task Given_LineageNotFound_When_Create_Then_ResourceNotFoundException()
  {
    FeatureModel payload = CreateEspritEveillePayload();
    Guid lineageId = Guid.Empty;

    var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await _lineageService.CreateOrReplaceFeatureAsync(lineageId, payload));
    Assert.Equal(Context.WorldUid, exception.WorldId);
    Assert.Equal(Lineage.ResourceKind, exception.ResourceKind);
    Assert.Equal(lineageId, exception.ResourceId);
    Assert.Equal("LineageId", exception.PropertyName);
  }

  private static FeatureModel CreateEspritEveillePayload() => new(
    " Esprit éveillé ",
    "   Le personnage ne peut être endormi de manière surnaturelle et il se voit conférer l’avantage à ses jets de sauvegarde contre les charmes. Il peut également s’harmoniser à un artefact magique supplémentaire.   ");

  private static FeatureModel CreateSensAffutesPayload() => new(
    " Sens affûtés ",
    "   Le personnage peut acquérir à rabais les talents Orientation et Perception.   ");

  private static void AssertEspritEveille(FeatureModel payload, LineageFeatureModel feature)
  {
    Assert.Equal(payload.Name.Trim(), feature.Name);
    Assert.Equal(payload.Content?.CleanTrim(), feature.Content);
  }

  private static void AssertSensAffutes(FeatureModel payload, LineageFeatureModel feature)
  {
    Assert.Equal(payload.Name.Trim(), feature.Name);
    Assert.Equal(payload.Content?.CleanTrim(), feature.Content);
  }
}

using Krakenar.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Specializations;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Specializations;
using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Specializations;

[Trait(Traits.Category, Categories.Integration)]
public class SpecializationIntegrationTests : IntegrationTests
{
  private readonly ISpecializationRepository _specializationRepository;
  private readonly ISpecializationService _specializationService;
  private readonly ITalentRepository _talentRepository;

  private Talent _armesDeTir = null!;
  private Talent _bricolageNaturel = null!;
  private Talent _naturalisme = null!;
  private Talent _survivalisme = null!;
  private Specialization _specialization = null!;

  public SpecializationIntegrationTests() : base()
  {
    _specializationRepository = ServiceProvider.GetRequiredService<ISpecializationRepository>();
    _specializationService = ServiceProvider.GetRequiredService<ISpecializationService>();
    _talentRepository = ServiceProvider.GetRequiredService<ITalentRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _armesDeTir = new Talent(World, new Tier(1), new Name("Armes de tir"));
    _bricolageNaturel = new Talent(World, new Tier(2), new Name("Bricolage naturel"));
    _naturalisme = new Talent(World, new Tier(1), new Name("Naturalisme"));
    _survivalisme = new Talent(World, new Tier(1), new Name("Survivalisme"));
    await _talentRepository.SaveAsync([_armesDeTir, _bricolageNaturel, _naturalisme, _survivalisme]);

    _specialization = new Specialization(World, new Tier(2), new Name("Specialization"), UserId);
    await _specializationRepository.SaveAsync(_specialization);
  }

  [Theory(DisplayName = "It should create a new specialization.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceSpecializationPayload payload = new()
    {
      Tier = 2,
      Name = "  Chasseur  ",
      Summary = "  Prédateur rusé, il traque ses proies et survit là où nul ne s’aventure.  ",
      Description = "  Arpenteur des forêts denses, des étendues sauvages, des déserts de glace ou même des profonds réseaux souterrains de cavernes, le chasseur se fond dans le décor des milieux naturels, silencieux comme le vent et connaisseur des sentiers cachés et des secrets de la nature. La plupart vit à l’écart des sociétés, à l’écart de la civilisation. Il est à la fois craint pour ses aptitudes rappelant celle d’une bête et respecté pour les services rendus lorsqu’il capture ou terrasse un terrible monstre terrorisant les communautés à proximité.  ",
      Requirements = new RequirementsPayload
      {
        TalentId = _survivalisme.EntityId,
        Other = ["Le personnage doit avoir acquis la spécialisation Éclaireur."]
      },
      Options = new OptionsPayload
      {
        TalentIds = [_armesDeTir.EntityId, _naturalisme.EntityId],
        Other = ["Un pouvoir de tiers 1 associé à un domaine d’animisme naturel."]
      },
      Doctrine = new DoctrinePayload
      {
        Name = "Pistage",
        Description = ["Le personnage acquiert les capacités suivantes."],
        DiscountedTalentIds = [_bricolageNaturel.EntityId],
        Features =
        [
          new FeatureModel("Fouler la terre", "Le personnage n’est pas ralenti par le terrain difficile naturel, ni par la végétation naturelle.\n\nIl se voit également conférer l’avantage à ses jets de sauvegarde contre les effets entravant ses mouvements provenant de la végétation surnaturelle ou d’une créature végétale.\n\nEnfin, lorsqu’il voyage à cadence prudente ou normale, il ne laisse aucune trace, sauf s’il le fait intentionnellement."),
          new FeatureModel("Proie privilégiée", "Lorsqu’il attaque sa cible privilégiée, il ajoute 1d8 points de dégâts supplémentaires plutôt que 1d4.\n\nÉgalement, il se voit conférer l’avantage à ses jets de sauvegarde contre les charmes et les effets de peur provenant de sa cible privilégiée."),
          new FeatureModel("Sens primitifs", "Lorsqu’il ne se trouve pas en milieu urbain, le personnage peut dépenser 15 points d’Énergie par une action afin d’observer son environnement.\n\nIl observe par exemple les traces au sol, de l’écorce griffée, une ou plusieurs carcasses, les mouvements de la faune ou encore le climat général.\n\nPour chacun des types suivants, il détermine si une créature appartenant à ce type se trouve à 1,5 kilomètres ou moins de sa position : aberration, célestiel, dragon, élémentaire, fée, infernal et mort-vivant.\n\nIl détermine seulement la présence de ces créatures, pas leur emplacement.")
        ]
      }
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceSpecializationResult result = await _specializationService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    SpecializationModel specialization = result.Specialization;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, specialization.Id);
    }
    Assert.Equal(2, specialization.Version);
    Assert.Equal(Actor, specialization.CreatedBy);
    Assert.Equal(DateTime.UtcNow, specialization.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, specialization.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, specialization.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Tier, specialization.Tier);
    Assert.Equal(payload.Name.Trim(), specialization.Name);
    Assert.Equal(payload.Summary?.Trim(), specialization.Summary);
    Assert.Equal(payload.Description?.Trim(), specialization.Description);
    Assert.Equal(payload.Requirements.TalentId, specialization.Requirements.Talent?.Id);
    Assert.Equal(payload.Requirements.Other, specialization.Requirements.Other);
    Assert.True(payload.Options.TalentIds.SequenceEqual(specialization.Options.Talents.Select(t => t.Id)));
    Assert.Equal(payload.Options.Other, specialization.Options.Other);

    Assert.NotNull(specialization.Doctrine);
    Assert.Equal(payload.Doctrine.Name, specialization.Doctrine.Name);
    Assert.Equal(payload.Doctrine.Description, specialization.Doctrine.Description);
    Assert.True(payload.Doctrine.DiscountedTalentIds.SequenceEqual(specialization.Doctrine.DiscountedTalents.Select(t => t.Id)));
    Assert.Equal(payload.Doctrine.Features, specialization.Doctrine.Features);
  }

  [Fact(DisplayName = "It should delete an existing specialization.")]
  public async Task Given_Exists_When_Delete_Then_Deleted()
  {
    Guid id = _specialization.EntityId;
    SpecializationModel? specialization = await _specializationService.DeleteAsync(id);
    Assert.NotNull(specialization);
    Assert.Equal(id, specialization.Id);
  }

  [Fact(DisplayName = "It should read an existing specialization.")]
  public async Task Given_Exists_When_Read_Then_Returned()
  {
    Guid id = _specialization.EntityId;
    SpecializationModel? specialization = await _specializationService.ReadAsync(id);
    Assert.NotNull(specialization);
    Assert.Equal(id, specialization.Id);
  }

  [Fact(DisplayName = "It should replace an existing specialization.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceSpecializationPayload payload = new()
    {
      Tier = 2,
      Name = "  Chasseur  ",
      Summary = "  Prédateur rusé, il traque ses proies et survit là où nul ne s’aventure.  ",
      Description = "  Arpenteur des forêts denses, des étendues sauvages, des déserts de glace ou même des profonds réseaux souterrains de cavernes, le chasseur se fond dans le décor des milieux naturels, silencieux comme le vent et connaisseur des sentiers cachés et des secrets de la nature. La plupart vit à l’écart des sociétés, à l’écart de la civilisation. Il est à la fois craint pour ses aptitudes rappelant celle d’une bête et respecté pour les services rendus lorsqu’il capture ou terrasse un terrible monstre terrorisant les communautés à proximité.\n\n  ",
      Requirements = new RequirementsPayload
      {
        TalentId = _survivalisme.EntityId,
        Other = ["Le personnage doit avoir acquis la spécialisation Éclaireur."]
      },
      Options = new OptionsPayload
      {
        TalentIds = [_armesDeTir.EntityId, _naturalisme.EntityId],
        Other = ["Un pouvoir de tiers 1 associé à un domaine d’animisme naturel."]
      },
      Doctrine = new DoctrinePayload
      {
        Name = "Pistage",
        Description = ["Le personnage acquiert les capacités suivantes."],
        DiscountedTalentIds = [_bricolageNaturel.EntityId],
        Features =
        [
          new FeatureModel("Fouler la terre", "Le personnage n’est pas ralenti par le terrain difficile naturel, ni par la végétation naturelle.\n\nIl se voit également conférer l’avantage à ses jets de sauvegarde contre les effets entravant ses mouvements provenant de la végétation surnaturelle ou d’une créature végétale.\n\nEnfin, lorsqu’il voyage à cadence prudente ou normale, il ne laisse aucune trace, sauf s’il le fait intentionnellement."),
          new FeatureModel("Proie privilégiée", "Lorsqu’il attaque sa cible privilégiée, il ajoute 1d8 points de dégâts supplémentaires plutôt que 1d4.\n\nÉgalement, il se voit conférer l’avantage à ses jets de sauvegarde contre les charmes et les effets de peur provenant de sa cible privilégiée."),
          new FeatureModel("Sens primitifs", "Lorsqu’il ne se trouve pas en milieu urbain, le personnage peut dépenser 15 points d’Énergie par une action afin d’observer son environnement.\n\nIl observe par exemple les traces au sol, de l’écorce griffée, une ou plusieurs carcasses, les mouvements de la faune ou encore le climat général.\n\nPour chacun des types suivants, il détermine si une créature appartenant à ce type se trouve à 1,5 kilomètres ou moins de sa position : aberration, célestiel, dragon, élémentaire, fée, infernal et mort-vivant.\n\nIl détermine seulement la présence de ces créatures, pas leur emplacement.")
        ]
      }
    };
    Guid id = _specialization.EntityId;

    CreateOrReplaceSpecializationResult result = await _specializationService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);
    SpecializationModel specialization = result.Specialization;

    Assert.Equal(id, specialization.Id);
    Assert.Equal(2, specialization.Version);
    Assert.Equal(Actor, specialization.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, specialization.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Tier, specialization.Tier);
    Assert.Equal(payload.Name.Trim(), specialization.Name);
    Assert.Equal(payload.Summary?.Trim(), specialization.Summary);
    Assert.Equal(payload.Description?.Trim(), specialization.Description);
    Assert.Equal(payload.Requirements.TalentId, specialization.Requirements.Talent?.Id);
    Assert.Equal(payload.Requirements.Other, specialization.Requirements.Other);
    Assert.True(payload.Options.TalentIds.SequenceEqual(specialization.Options.Talents.Select(t => t.Id)));
    Assert.Equal(payload.Options.Other, specialization.Options.Other);

    Assert.NotNull(specialization.Doctrine);
    Assert.Equal(payload.Doctrine.Name, specialization.Doctrine.Name);
    Assert.Equal(payload.Doctrine.Description, specialization.Doctrine.Description);
    Assert.True(payload.Doctrine.DiscountedTalentIds.SequenceEqual(specialization.Doctrine.DiscountedTalents.Select(t => t.Id)));
    Assert.Equal(payload.Doctrine.Features, specialization.Doctrine.Features);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Search_Then_Results()
  {
    Specialization eclaireur = new(World, new Tier(1), new Name("Éclaireur"));
    Specialization archer = new(World, new Tier(2), new Name("Archer"));
    Specialization druide = new(World, new Tier(2), new Name("Druide"));
    druide.Summary = new Summary("Gardien de la nature, maître des formes sauvages et du verbe sacré.");
    druide.Update(UserId);
    Specialization mage = new(World, new Tier(2), new Name("Mage"));
    mage.SetDoctrine(new Name("Magie raffinée"), [], [], []);
    mage.Update(UserId);
    await _specializationRepository.SaveAsync([eclaireur, archer, druide, mage]);

    SearchSpecializationsPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Tiers.Add(2);
    payload.Ids.AddRange([eclaireur.EntityId, archer.EntityId, druide.EntityId, mage.EntityId, Guid.Empty]);
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.AddRange([new SearchTerm("Spec%"), new SearchTerm("%reur"), new SearchTerm("%nature%"), new SearchTerm("%ff%")]);
    payload.Sort.Add(new SpecializationSortOption(SpecializationSort.Name));

    SearchResults<SpecializationModel> results = await _specializationService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    SpecializationModel result = Assert.Single(results.Items);
    Assert.Equal(mage.EntityId, result.Id);
  }

  [Fact(DisplayName = "It should update an existing specialization.")]
  public async Task Given_Exists_When_Update_ThenUpdated()
  {
    Guid id = _specialization.EntityId;
    UpdateSpecializationPayload payload = new()
    {
      Name = "  Chasseur  ",
      Summary = new Update<string>("  Prédateur rusé, il traque ses proies et survit là où nul ne s’aventure.  "),
      Description = new Update<string>("  Arpenteur des forêts denses, des étendues sauvages, des déserts de glace ou même des profonds réseaux souterrains de cavernes, le chasseur se fond dans le décor des milieux naturels, silencieux comme le vent et connaisseur des sentiers cachés et des secrets de la nature. La plupart vit à l’écart des sociétés, à l’écart de la civilisation. Il est à la fois craint pour ses aptitudes rappelant celle d’une bête et respecté pour les services rendus lorsqu’il capture ou terrasse un terrible monstre terrorisant les communautés à proximité.  "),
      Requirements = new RequirementsPayload
      {
        TalentId = _survivalisme.EntityId,
        Other = ["Le personnage doit avoir acquis la spécialisation Éclaireur."]
      },
      Options = new OptionsPayload
      {
        TalentIds = [_armesDeTir.EntityId, _naturalisme.EntityId],
        Other = ["Un pouvoir de tiers 1 associé à un domaine d’animisme naturel."]
      },
      Doctrine = new Update<DoctrinePayload>(new DoctrinePayload
      {
        Name = "Pistage",
        Description = ["Le personnage acquiert les capacités suivantes."],
        DiscountedTalentIds = [_bricolageNaturel.EntityId],
        Features =
        [
          new FeatureModel("Fouler la terre", "Le personnage n’est pas ralenti par le terrain difficile naturel, ni par la végétation naturelle.\n\nIl se voit également conférer l’avantage à ses jets de sauvegarde contre les effets entravant ses mouvements provenant de la végétation surnaturelle ou d’une créature végétale.\n\nEnfin, lorsqu’il voyage à cadence prudente ou normale, il ne laisse aucune trace, sauf s’il le fait intentionnellement."),
          new FeatureModel("Proie privilégiée", "Lorsqu’il attaque sa cible privilégiée, il ajoute 1d8 points de dégâts supplémentaires plutôt que 1d4.\n\nÉgalement, il se voit conférer l’avantage à ses jets de sauvegarde contre les charmes et les effets de peur provenant de sa cible privilégiée."),
          new FeatureModel("Sens primitifs", "Lorsqu’il ne se trouve pas en milieu urbain, le personnage peut dépenser 15 points d’Énergie par une action afin d’observer son environnement.\n\nIl observe par exemple les traces au sol, de l’écorce griffée, une ou plusieurs carcasses, les mouvements de la faune ou encore le climat général.\n\nPour chacun des types suivants, il détermine si une créature appartenant à ce type se trouve à 1,5 kilomètres ou moins de sa position : aberration, célestiel, dragon, élémentaire, fée, infernal et mort-vivant.\n\nIl détermine seulement la présence de ces créatures, pas leur emplacement.")
        ]
      })
    };

    SpecializationModel? specialization = await _specializationService.UpdateAsync(id, payload);
    Assert.NotNull(specialization);

    Assert.Equal(id, specialization.Id);
    Assert.Equal(2, specialization.Version);
    Assert.Equal(Actor, specialization.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, specialization.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), specialization.Name);
    Assert.Equal(payload.Summary.Value?.Trim(), specialization.Summary);
    Assert.Equal(payload.Description.Value?.Trim(), specialization.Description);
    Assert.Equal(payload.Requirements.TalentId, specialization.Requirements.Talent?.Id);
    Assert.Equal(payload.Requirements.Other, specialization.Requirements.Other);
    Assert.True(payload.Options.TalentIds.SequenceEqual(specialization.Options.Talents.Select(t => t.Id)));
    Assert.Equal(payload.Options.Other, specialization.Options.Other);

    Assert.NotNull(payload.Doctrine.Value);
    Assert.NotNull(specialization.Doctrine);
    Assert.Equal(payload.Doctrine.Value.Name, specialization.Doctrine.Name);
    Assert.Equal(payload.Doctrine.Value.Description, specialization.Doctrine.Description);
    Assert.True(payload.Doctrine.Value.DiscountedTalentIds.SequenceEqual(specialization.Doctrine.DiscountedTalents.Select(t => t.Id)));
    Assert.Equal(payload.Doctrine.Value.Features, specialization.Doctrine.Features);
  }
}

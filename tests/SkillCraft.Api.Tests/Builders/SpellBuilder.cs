using Bogus;
using SkillCraft.Api.Core.Spells;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Builders;

public interface ISpellBuilder
{
  ISpellBuilder WithId(Guid id);
  ISpellBuilder WithWorld(World? world);
  ISpellBuilder WithTier(int tier);
  ISpellBuilder WithName(string name);
  ISpellBuilder WithSummary(string? summary);
  ISpellBuilder WithContent(string? content);

  Spell Build();
}

public class SpellBuilder : ISpellBuilder
{
  private readonly Faker _faker;

  private string? _content = null;
  private Guid? _id = null;
  private string _name = "Spell";
  private string? _summary = null;
  private int _tier = 0;
  private World? _world = null;

  public SpellBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public ISpellBuilder WithId(Guid id)
  {
    _id = id;
    return this;
  }

  public ISpellBuilder WithWorld(World? world)
  {
    _world = world;
    return this;
  }

  public ISpellBuilder WithTier(int tier)
  {
    _tier = tier;
    return this;
  }

  public ISpellBuilder WithName(string name)
  {
    _name = name;
    return this;
  }

  public ISpellBuilder WithSummary(string? summary)
  {
    _summary = summary;
    return this;
  }

  public ISpellBuilder WithContent(string? content)
  {
    _content = content;
    return this;
  }

  public Spell Build()
  {
    World world = _world ?? new WorldBuilder(_faker).Build();
    return new Spell(world, _tier, _id)
    {
      Name = _name,
      Summary = _summary,
      Content = _content
    };
  }

  public static Spell ProtectionContreLaMagie(Faker? faker = null, World? world = null) => new SpellBuilder(faker)
    .WithWorld(world)
    .WithTier(1)
    .WithName("Protection contre la magie")
    .WithSummary("Détection, dissipation et interruption des effets magiques adverses.")
    .WithContent("Pouvoir défensif et utilitaire permettant de détecter la magie, dissiper les effets surnaturels actifs et interrompre les incantations ennemies en réaction.")
    .Build();

  public static Spell Lumiere(Faker? faker = null, World? world = null) => new SpellBuilder(faker)
    .WithWorld(world)
    .WithName("Lumière")
    .WithSummary("Illumine un objet ou crée une sphère de lumière mobile.")
    .WithContent("Ce pouvoir éclaire durablement un objet ou génère une large sphère lumineuse, offrant clarté et pénombre pour révéler l’environnement selon le niveau.")
    .Build();

  public static Spell Guerison(Faker? faker = null, World? world = null) => new SpellBuilder(faker)
    .WithWorld(world)
    .WithTier(1)
    .WithName("Guérison")
    .WithSummary("Guérit une ou plusieurs créatures au toucher ou par un mot à distance.")
    .WithContent("Pouvoir de guérison permettant de restaurer la Vitalité d’une ou plusieurs créatures, au toucher ou à distance. Les morts-vivants et les constructions ne sont pas affectés par ce pouvoir.")
    .Build();
}

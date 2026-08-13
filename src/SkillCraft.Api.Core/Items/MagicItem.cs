namespace SkillCraft.Api.Core.Items;

public record MagicItem
{
  public Attunement? Attunement { get; }

  public MagicItem(Attunement? attunement = null)
  {
    Attunement = attunement;
  }
}

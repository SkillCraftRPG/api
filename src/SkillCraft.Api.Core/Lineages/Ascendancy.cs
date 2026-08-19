namespace SkillCraft.Api.Core.Lineages;

public record Ascendancy // TODO(fpion): use in character constructor
{
  public Lineage Species { get; }
  public Lineage? Ethnicity { get; }

  public int ExtraLanguages
  {
    get
    {
      int extra = Species.Languages.Extra;
      if (Ethnicity is not null)
      {
        extra += Ethnicity.Languages.Extra;
      }
      return extra;
    }
  }

  public Ascendancy(Lineage species, Lineage? ethnicity = null)
  {
    if (species.ParentId.HasValue)
    {
      throw new ArgumentException("The species cannot have a parent.", nameof(species));
    }

    if (ethnicity is not null && ethnicity.ParentId != species.Id)
    {
      throw new ArgumentException("The ethnicity must belong to the species.", nameof(ethnicity));
    }

    Species = species;
    Ethnicity = ethnicity;
  }
}

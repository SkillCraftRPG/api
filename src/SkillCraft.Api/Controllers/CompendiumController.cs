using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Core.Educations.Models;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Core.Scripts.Models;
using SkillCraft.Api.Core.Talents.Models;
using SkillCraft.Api.Infrastructure.Compendium;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Route("compendium")]
public class CompendiumController : Controller
{
  private readonly ICompendiumService _compendium;

  public CompendiumController(ICompendiumService compendium)
  {
    _compendium = compendium;
  }

  [HttpGet("castes")]
  public async Task<ActionResult<SearchResults<CasteModel>>> GetCastesAsync(CancellationToken cancellationToken)
  {
    SearchResults<CasteModel> castes = await _compendium.GetCastesAsync(cancellationToken);
    return Ok(castes);
  }

  [HttpGet("customizations")]
  public async Task<ActionResult<SearchResults<CustomizationModel>>> GetCustomizationsAsync(CancellationToken cancellationToken)
  {
    SearchResults<CustomizationModel> customizations = await _compendium.GetCustomizationsAsync(cancellationToken);
    return Ok(customizations);
  }

  [HttpGet("educations")]
  public async Task<ActionResult<SearchResults<EducationModel>>> GetEducationsAsync(CancellationToken cancellationToken)
  {
    SearchResults<EducationModel> customizations = await _compendium.GetEducationsAsync(cancellationToken);
    return Ok(customizations);
  }

  [HttpGet("languages")]
  public async Task<ActionResult<SearchResults<LanguageModel>>> GetLanguagesAsync(CancellationToken cancellationToken)
  {
    SearchResults<LanguageModel> languages = await _compendium.GetLanguagesAsync(cancellationToken);
    return Ok(languages);
  }

  [HttpGet("scripts")]
  public async Task<ActionResult<SearchResults<ScriptModel>>> GetScriptsAsync(CancellationToken cancellationToken)
  {
    SearchResults<ScriptModel> scripts = await _compendium.GetScriptsAsync(cancellationToken);
    return Ok(scripts);
  }

  [HttpGet("talents")]
  public async Task<ActionResult<SearchResults<TalentModel>>> GetTalentsAsync(CancellationToken cancellationToken)
  {
    SearchResults<TalentModel> talents = await _compendium.GetTalentsAsync(cancellationToken);
    return Ok(talents);
  }
}

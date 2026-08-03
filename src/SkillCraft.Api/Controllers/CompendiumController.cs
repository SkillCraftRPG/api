using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Infrastructure.Compendium;

namespace SkillCraft.Api.Controllers;

[ApiController]
[Authorize]
[Route("compendium")]
public class CompendiumController : Controller
{
  private readonly ICompendiumService _compendium;

  public CompendiumController(ICompendiumService compendium)
  {
    _compendium = compendium;
  }

  [HttpGet("customizations")]
  public async Task<ActionResult<SearchResults<CustomizationModel>>> GetCustomizationsAsync(CancellationToken cancellationToken)
  {
    SearchResults<CustomizationModel> customizations = await _compendium.GetCustomizationsAsync(cancellationToken);
    return Ok(customizations);
  }

  [HttpGet("languages")]
  public async Task<ActionResult<SearchResults<LanguageModel>>> GetLanguagesAsync(CancellationToken cancellationToken)
  {
    SearchResults<LanguageModel> languages = await _compendium.GetLanguagesAsync(cancellationToken);
    return Ok(languages);
  }
}

using Inovatec.OfficeManagementTool.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class TranslateController : Controller
    {
        [HttpGet]
        [Route("{lang}")]
        public IActionResult GetTranslationsForLanguage([FromRoute] string lang)
        {
            if(lang != "en" && lang != "sr")
            {
                return NotFound(new { message = "Translation for this language doesn't exist." });
            }

            return Ok(LocalizationManager.GetJsonResources(lang));
        }
    }
}

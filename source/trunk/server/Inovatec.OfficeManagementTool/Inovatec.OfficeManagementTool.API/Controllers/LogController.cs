using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [Authorize(Roles = Role.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogUI _logUI;

        public LogController(ILogUI logUI)
        {
            _logUI = logUI;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] LogsFilterRequest logsFilterRequest)
        {
            return Ok(await _logUI.GetLogs(logsFilterRequest));
        }
    }
}

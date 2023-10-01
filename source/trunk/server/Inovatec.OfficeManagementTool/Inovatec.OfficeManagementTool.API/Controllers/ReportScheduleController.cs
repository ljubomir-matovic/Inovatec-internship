using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [Authorize(Roles = Role.HR)]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportScheduleController : Controller
    {
        private readonly IReportScheduleUI _reportScheduleUI;

        public ReportScheduleController(IReportScheduleUI reportScheduleUI)
        {
            _reportScheduleUI = reportScheduleUI;
        }

        [HttpGet]
        public async Task<IActionResult> GetReportSchedulePage([FromQuery] ReportScheduleFilterRequest filterRequest)
        {
            return Ok(await _reportScheduleUI.GetReportSchedulePage(filterRequest));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetReportScheduleById([FromRoute] int id)
        {
            return Ok(await _reportScheduleUI.GetReportScheduleById(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddReportSchedule([FromBody] ReportScheduleCreateRequest reportScheduleCreateRequest)
        {
            return Ok(await _reportScheduleUI.Insert(reportScheduleCreateRequest));
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateReportSchedule([FromBody] UpdateReportScheduleRequest updateReportScheduleRequest)
        {
            return Ok(await _reportScheduleUI.Update(updateReportScheduleRequest));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteReportSchedule([FromRoute] long id)
        {
            return Ok(await _reportScheduleUI.Delete(id));
        }
    }
}

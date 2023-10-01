using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [Authorize(Roles = Role.Admin + "," + Role.HR + "," + Role.OrdinaryEmployee)]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        private readonly IReportUI _reportUI;

        public ReportController(IReportUI reportUI)
        {
            _reportUI = reportUI;
        }

        [HttpGet]
        public async Task<IActionResult> GetReports([FromQuery] ReportFilterRequest reportFilterRequest)
        {
            return Ok(await _reportUI.GetReports(reportFilterRequest));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetReport([FromRoute] int id)
        {
            return Ok(await _reportUI.GetReportById(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddReport(ReportCreateRequest reportCreateRequest)
        {
            return Ok(await _reportUI.AddReport(reportCreateRequest));
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateReport([FromBody] UpdateReportRequest updateReportRequest)
        {
            return Ok(await _reportUI.UpdateReport(updateReportRequest));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteReport([FromRoute] long id)
        {
            return Ok(await _reportUI.DeleteReport(id));
        }
    } 
}
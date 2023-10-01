using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [Authorize(Roles = Role.HR + "," + Role.Admin)]
    [ApiController]
    [Route("api/[controller]")]
    public class OfficeController : Controller
    {
        private readonly IOfficeUI _officeUI;
        
        public OfficeController(IOfficeUI officeUI)
        {
            _officeUI = officeUI;
        }

        [HttpGet]
        public async Task<IActionResult> GetOfficePage([FromQuery] OfficeFilterRequest filterRequest)
        {
            return Ok(await _officeUI.GetOfficePage(filterRequest));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetOfficeById([FromRoute] int id)
        {
            return Ok(await _officeUI.GetOfficeById(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddOffice([FromBody] OfficeCreateRequest officeCreateRequest)
        {
            return Ok(await _officeUI.Insert(officeCreateRequest));
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateOfficeRequest updateOfficeRequest)
        {
            return Ok(await _officeUI.Update(updateOfficeRequest));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] long id)
        {
            return Ok(await _officeUI.Delete(id));
        }
    }
}

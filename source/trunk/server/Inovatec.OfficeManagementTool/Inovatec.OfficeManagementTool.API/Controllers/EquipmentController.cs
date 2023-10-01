using Inovatec.OfficeManagementTool.Common.Services.UserService;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.ModelBinding;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [Authorize(Roles = Role.HR + "," + Role.Admin)]
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController : Controller
    {
        private readonly IEquipmentUI _equipmentUI;
        private readonly IUserService _userService;

        public EquipmentController(IEquipmentUI equipmentUI, IUserService userService) 
        {
            _equipmentUI = equipmentUI;
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> GetEquipmentsForCurrentUser([FromQuery] EquipmentFilterRequest queryParams)
        {
            if (queryParams.PageNumber < 1)
            {
                ModelStateDictionary dict = new ModelStateDictionary();
                dict.AddModelError("pageNumber", "pageNumber must be whole number greater than 0");
                return BadRequest(dict);
            }

            var page = await _equipmentUI.GetEquipments(queryParams, true);

            if (page.Data.Count == 0)
            {
                return NotFound(new { message = "Page not exists" });
            }

            return Ok(page);
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetEquipments([FromQuery] EquipmentFilterRequest queryParams)
        {

            if (queryParams.PageNumber < 1)
            {
                ModelStateDictionary dict = new ModelStateDictionary();
                dict.AddModelError("pageNumber", "pageNumber must be whole number greater than 0");
                return BadRequest(dict);
            }
            

            var page = await _equipmentUI.GetEquipments(queryParams);

            if (page.Data.Count == 0)
            {
                return NotFound(new { message = "Page not exists" });
            }

            return Ok(page);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateEquipment([FromBody] EquipmentCreateRequest requestBody)
        {
            var result = await _equipmentUI.Insert(requestBody);
            
            if (result.ActionSuccess)
            {
                return Ok(result);
            }

            return Conflict(result.Errors);
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> UpdateEquipment([FromBody] EquipmentUpdateRequest requestBody)
        {
            var result = await _equipmentUI.Update(requestBody);

            if (result.ActionSuccess)
            {
                return Ok(result);
            }

            return Conflict(result.Errors);
        }

        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> DeleteSelectedEquipments([FromBody] List<long> ids)
        {
            var result = await _equipmentUI.DeleteMore(ids);

            if (result.ActionSuccess)
            {
                return Ok(result);
            }

            return StatusCode(500, result.Errors);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteEquipment([FromRoute] long id)
        {
            var result = await _equipmentUI.Delete(id);

            if (result.ActionSuccess)
            {
                return Ok(result);
            }

            return StatusCode(500, result.Errors);
        }
    }
}

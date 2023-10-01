using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserUI _userUI;
        public UserController(IUserUI userUI)
        {
            _userUI = userUI;
        }

        [Authorize(Roles = Role.Admin + "," + Role.HR)]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetUsers([FromQuery] UserFilterRequest queryParams)
        {
            if(queryParams.PageNumber < 1) 
            {
                ModelStateDictionary dict = new ModelStateDictionary();
                dict.AddModelError("pageNumber", "pageNumber must be whole number greater than 0");
                return BadRequest(dict);
            }

            var page = await _userUI.GetUsers(queryParams);

            if(page.Data.Count == 0) 
            {
                return NotFound(new { message = "Page not exists" });
            }

            return Ok(page);
        }

        [Authorize(Roles = Role.Admin + "," + Role.HR)]
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest requestBody)
        {
            var result = await _userUI.Insert(requestBody);

            if(result.ActionSuccess)
            {
                return Ok(result);
            }

            return Conflict(result.Errors);
        }

        [Authorize(Roles = Role.Admin + "," + Role.HR)]
        [HttpGet]
        [Route("csvTemplate")]
        public async Task<IActionResult> AddUsersFromCSV()
        {
            return Ok(await _userUI.GetCSVTemplate());
        }

        [Authorize(Roles = Role.Admin + "," + Role.HR)]
        [HttpPost]
        [Route("fromCSV")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddUsersFromCSV([FromForm] CSVUploadRequest uploadRequest)
        {
            return Ok(await _userUI.AddUsersFromCSV(uploadRequest));
        }

        [Authorize(Roles = Role.Admin + "," + Role.HR)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            var user = await _userUI.GetUserByIdForAdmin(id);

            if(user == null) 
            {
                ActionResultResponse<object> result = new ActionResultResponse<object>();
                result.ActionSuccess = false;
                result.Errors.Add(string.Format("User with id {0} doesn\'t exist.", id));
                return NotFound(result);
            }

            return Ok(user);
        }

        [Authorize(Roles = Role.Admin + "," + Role.HR)]
        [HttpPut]
        [Route("")]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel requestBody)
        {
            var result = await _userUI.Update(requestBody);

            if (result.ActionSuccess)
            {
                return Ok(result);
            }

            return Conflict(result.Errors);
        }

        [Authorize(Roles = Role.Admin + "," + Role.HR)]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] long id)
        {
            var result = await _userUI.Delete(id);

            if (result.ActionSuccess)
            {
                return Ok(result);
            }

            return StatusCode(500,result.Errors);
        }

        [Authorize]
        [HttpGet]
        [Route("personal-data")]
        public async Task<IActionResult> GetPersonalData()
        {
            return Ok(await _userUI.GetPersonalData());
        }

        [Authorize]
        [HttpPut]
        [Route("personal-data")]
        public async Task<IActionResult> ChangeFirstOrLastName([FromBody] ChangePersonalData requestBody)
        {
            return Ok( await _userUI.ChangeFirstOrLastName(requestBody));
        }

        [Authorize]
        [HttpPut]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest requestBody)
        {
            return Ok(await _userUI.ChangePassword(requestBody));
        }
    } 
}
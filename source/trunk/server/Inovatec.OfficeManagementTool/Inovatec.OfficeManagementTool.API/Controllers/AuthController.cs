using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserUI _userUI;

        public AuthController(IUserUI userUI)
        {
            _userUI = userUI;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse?>> Login([FromBody] LoginRequest loginRequest)
        {
            ActionResultResponse<LoginResponse?> result = await _userUI.LoginUser(loginRequest);
            return Ok(result);
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> CreateResetToken([FromBody] ForgotPasswordRequest requestBody)
        {
            var result = await _userUI.CreateResetToken(requestBody.Email);

            if (result.ActionSuccess)
            {
                return Ok(result);
            }

            return StatusCode(404, result.Errors);
        }

        [HttpPut]
        [Route("reset-password")]
        public async Task<IActionResult> ResetToken([FromBody] ResetPasswordRequest requestBody)
        {
            var result = await _userUI.ResetPassword(requestBody.Token, requestBody.Password);

            if (result.ActionSuccess)
            {
                return Ok(result);
            }

            return StatusCode(404, result.Errors);
        }
    }
}
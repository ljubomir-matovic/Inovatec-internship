using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [Authorize(Roles = Role.Admin + "," + Role.HR + "," + Role.OrdinaryEmployee)]
    [ApiController]
    [Route("/api/[controller]")]
    public class CommentController : Controller
    {
        private readonly ICommentUI _commentUI;

        public CommentController(ICommentUI commentUI)
        {
            _commentUI = commentUI;
        }

        [HttpGet]
        public async Task<IActionResult> GetComments([FromQuery] CommentFilterRequest commentFilterRequest)
        {
            return Ok(await _commentUI.GetComments(commentFilterRequest));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetComment([FromRoute] int id)
        {
            return Ok(await _commentUI.GetCommentById(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CommentCreateRequest commentCreateRequest)
        {
            return Ok(await _commentUI.Insert(commentCreateRequest));
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentRequest updatedComment)
        {
            return Ok(await _commentUI.Update(updatedComment));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] long id)
        {
            return Ok(await _commentUI.Delete(id));
        }
    } 
}
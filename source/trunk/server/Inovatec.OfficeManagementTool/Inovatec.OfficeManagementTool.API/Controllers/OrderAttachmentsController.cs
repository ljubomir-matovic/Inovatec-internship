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
    public class OrderAttachmentsController : Controller
    {
        private readonly IOrderAttachmentUI _orderAttachmentUI;

        public OrderAttachmentsController(IOrderAttachmentUI orderAttachmentUI)
        {
            _orderAttachmentUI=orderAttachmentUI;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetFileById([FromRoute] long id)
        {
            return Ok(await _orderAttachmentUI.GetAttachmentById(id));
        }

        [HttpGet]
        [Route("order/{id}")]
        public async Task<IActionResult> GetAttachmentsForOrder([FromRoute] long id)
        {
            return Ok(await _orderAttachmentUI.GetAttachmentsForOrder(id));
        }

        [HttpPost]
        [Route("")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadRequest requestBody)
        {
            return Ok(await _orderAttachmentUI.UploadFile(requestBody));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteFile([FromRoute] long id)
        {
            return Ok(await _orderAttachmentUI.Delete(id));
        }
    }
}

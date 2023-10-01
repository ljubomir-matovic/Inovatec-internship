using Inovatec.OfficeManagementTool.ImplementationsUI;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inovatec.OfficeManagementTool.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        private readonly INotificationUI _notificationUI;

        public NotificationController(INotificationUI notificationUI)
        {
            _notificationUI = notificationUI;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetNotifications([FromQuery] NotificationFilterRequest queryParams)
        {
            return Ok(await _notificationUI.GetNotifications(queryParams));
        }

        [HttpGet]
        [Route("unreadNumber")]
        public async Task<IActionResult> GetUnreadNotificationNumber()
        {
            return Ok(await _notificationUI.GetUnreadNotificationNumber());
        }

        [HttpPut]
        [Route("markAsRead/{id}")]
        public async Task<IActionResult> MarkNotificationAsRead([FromRoute] long id)
        {
            return Ok(await _notificationUI.MarkNotificationAsRead(id));
        }
    }
}

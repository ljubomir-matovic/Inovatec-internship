using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Inovatec.OfficeManagementTool.Hubs
{
    [Authorize]
    public class NotificationHub : BaseHub 
    {
        public const string HRGroupPrefix = "HROffice";
        public const string OfficeGroupPrefix = "OfficeOrders";
        public string OfficeGroup => OfficeGroupPrefix + GetClaim(ClaimName.OfficeId);

        public string HRGroup => HRGroupPrefix + GetClaim(ClaimName.OfficeId);
        private readonly INotificationBL _notificationBL;

        public NotificationHub(INotificationBL notificationBL)
        {
            _notificationBL = notificationBL;
        }

        public override async Task OnConnectedAsync()
        {
            string role = GetClaim(ClaimName.Role);

            if (role == Role.HR)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, HRGroup);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, OfficeGroup);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string role = GetClaim(ClaimName.Role);

            if (role == Role.HR)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, HRGroup);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, OfficeGroup);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task NotificationIsRead(long notificationId)
        {
            var notification = await _notificationBL.GetById(notificationId);

            if (notification == null)
                return;

            await Clients.Group(CurrentUser).SendAsync("notificationIsRead", notification.Id, notification.Data, notification.Description, notification.DateCreated);
        }

        public async Task MarkAllAsRead()
        {
            var unread = await _notificationBL.GetNotifications(new NotificationFilterRequest { IsRead = false });
            await _notificationBL.MarkAllAsRead(long.Parse(GetClaim(ClaimName.Id)));
            //await Clients.Group(CurrentUser).SendAsync("MarkAllAsRead");
            foreach(var notification in unread.Data)
            {
                await Clients.Group(CurrentUser).SendAsync("notificationIsRead", notification.Id, notification.Data, notification.Description, notification.DateCreated);
            }
        }
    }
}
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class NotificationUI : INotificationUI
    {
        private readonly INotificationBL _notificationBL;

        public NotificationUI(INotificationBL notificationBL)
        {
            _notificationBL = notificationBL;
        }

        public async Task<DataPage<NotificationViewModel>> GetNotifications(NotificationFilterRequest filterRequest)
        {
            return await _notificationBL.GetNotifications(filterRequest);
        }

        public async Task<int> GetUnreadNotificationNumber()
        {
            return await _notificationBL.GetUnreadNotificationNumber();
        }

        public async Task<ActionResultResponse> MarkNotificationAsRead(long id)
        {
            return await _notificationBL.MarkNotificationAsRead(id);
        }
    }
}
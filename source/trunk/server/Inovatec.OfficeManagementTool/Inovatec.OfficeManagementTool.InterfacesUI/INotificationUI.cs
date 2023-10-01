using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesUI
{
    public interface INotificationUI
    {
        Task<DataPage<NotificationViewModel>> GetNotifications(NotificationFilterRequest filterRequest);
        Task<int> GetUnreadNotificationNumber();
        Task<ActionResultResponse> MarkNotificationAsRead(long id);
    }
}
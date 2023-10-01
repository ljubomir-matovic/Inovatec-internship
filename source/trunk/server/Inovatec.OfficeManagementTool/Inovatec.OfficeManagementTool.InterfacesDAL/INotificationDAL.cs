using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface INotificationDAL : IBaseDAL<Notification>
    {
        Task<(List<NotificationViewModel>, int, bool)> GetNotifications(NotificationFilterRequest filterRequest);
        Task<int> GetUnreadNotificationNumber(long id);
    }
}
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface INotificationBL
    {
        Task<DataPage<NotificationViewModel>> GetNotifications(NotificationFilterRequest filterRequest);
        Task CreateNotificationForNewOrder(long orderId);
        Task CreateNotificationForChangedStateOfOrder(long officeId, long orderId, OrderState oldState, OrderState newState);
        Task<int> GetUnreadNotificationNumber();
        Task<ActionResultResponse> MarkNotificationAsRead(long id);
        Task<Notification?> GetById(long notificationId);
        Task MarkAllAsRead(long userId);
        Task ChangedEquipmentOrder(UpdateEquipmentOrderRequest updateEquipmentOrderRequest, long ownerId, string ownerEmail);
    }
}
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface IOrderItemDAL: IBaseDAL<OrderItem>
    {
        Task<(List<OrderItemViewModel>, long)> GetOrderItemsForOrder(long orderId, EquipmentFilterRequest filterRequest);
        Task<bool> OrderItemExists(long orderId, long itemId);
    }
}
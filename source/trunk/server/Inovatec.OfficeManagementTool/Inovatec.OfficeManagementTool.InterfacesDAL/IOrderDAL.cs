using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface IOrderDAL : IBaseDAL<Order>
    {
        Task AddOrderItem(OrderItem item);
        Task<int> CreateOrder(long officeId, object orderId);
        Task<(List<Order>, long)> GetOrdersPage(OrderFilterRequest orderFilterRequest);
    }
}
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface IOrderRequestDAL : IBaseDAL<OrderRequest>
    {
        Task<(List<OrderRequest>, long)> GetActiveOrderPage(OrderFilterRequest orderFilterRequest);

        Task<OrderRequest?> GetOrderRequest(long id);

        Task<(List<OrderViewModelGroupByItem>, long)> GetActiveOrdersGroupedByItem(OrderFilterRequest orderFilterRequest);

        Task<(List<OrderRequest>, long)> GetReportPage(OrderFilterRequest orderFilterRequest);

        Task<(List<OrderRequest>, long)> GetEquipmentPage(OrderFilterRequest orderFilterRequest);

        Task<OrderRequest?> GetFullOrderById(long id);

        Task<List<SnacksOrderViewModel>> GetSnacksOrderReport(long officeId);

        Task<List<OrderRequest>> UnacceptedOrders();

        Task<List<OrderItem>> GetOrderItems();
    }
}
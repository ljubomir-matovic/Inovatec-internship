using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesUI
{
    public interface IOrderUI
    {
        Task<Order?> GetOrderById(int id);
        Task<ActionResultResponse<string>> CreateOrderFromCart();
        Task<ActionResultResponse<string>> ChangeState(OrderUpdateRequest requestBody);
        Task<ActionResultResponse<DataPage<Order>>> GetOrders(OrderFilterRequest orderFilterRequest);
    }
}
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface IOrderBL
    {
        
        Task<Order?> GetOrderById(int id);

        Task<ActionResultResponse<string>> Delete(long id);

        Task<ActionResultResponse<string>> CreateOrderFromCart();

        Task<ActionResultResponse<DataPage<Order>>> GetOrders(OrderFilterRequest orderFilterRequest);

        Task<ActionResultResponse<string>> ChangeState(OrderUpdateRequest requestBody);
    }
}
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface IOrderItemBL
    {
        Task<(List<OrderItemViewModel>, long)> GetOrderItemsForOrder(long orderId, EquipmentFilterRequest filterRequest);
        Task<ActionResultResponse<string>> Insert(OrderItemCreateRequest entity);
        Task<ActionResultResponse<string>> Update(OrderItemCreateRequest entity);
        Task<ActionResultResponse<string>> DeleteMore(List<long> ids);
        Task<ActionResultResponse<string>> Delete(long id);
    }
}
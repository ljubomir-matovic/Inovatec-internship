using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesUI
{
    public interface IOrderItemUI
    {
        Task<DataPage<OrderItemViewModel>> GetOrderItemsForOrder(long orderId, EquipmentFilterRequest filterRequest);
        Task<ActionResultResponse<string>> Insert(OrderItemCreateRequest entity);
        Task<ActionResultResponse<string>> Update(OrderItemCreateRequest entity);
        Task<ActionResultResponse<string>> DeleteMore(List<long> ids);
        Task<ActionResultResponse<string>> Delete(long id);
    }
}
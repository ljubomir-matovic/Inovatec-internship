using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesUI
{
    public interface IOrderRequestUI
    {
        Task<ActionResultResponse<OrderViewModel?>> GetOrderById(int id);
        Task<ActionResultResponse<DataPage<OrderViewModel>>> GetActiveOrders(OrderFilterRequest orderFilterRequest);
        Task<ActionResultResponse<DataPage<OrderViewModelGroupByItem>>> GetActiveOrdersGroupedByItem(OrderFilterRequest orderFilterRequest);
        Task<ActionResultResponse<string>> AddSnackOrder(SnackCreateRequest snackCreateRequest);
        Task<ActionResultResponse<string>> UpdateSnackOrder(SnackUpdateRequest snackUpdateRequest);
        Task<ActionResultResponse<string>> Delete(long id);
        Task<ActionResultResponse<DataPage<OrderViewModel>>> GetEquipmentOrders(OrderFilterRequest orderFilterRequest);
        Task<ActionResultResponse<string>> AddEquipmentOrder(EquipmentOrderCreateRequest equipmentOrderCreateRequest);
        Task<ActionResultResponse<string>> UpdateEquipmentOrder(UpdateEquipmentOrderRequest updateEquipmentOrderRequest);
    }
}
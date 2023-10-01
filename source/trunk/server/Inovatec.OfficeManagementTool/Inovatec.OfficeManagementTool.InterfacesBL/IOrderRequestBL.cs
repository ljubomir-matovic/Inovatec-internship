using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface IOrderRequestBL
    {
        Task<ActionResultResponse<OrderViewModel?>> GetOrderById(int id);

        Task<ActionResultResponse<DataPage<OrderViewModel>>> GetActiveOrders(OrderFilterRequest orderFilterRequest);

        Task<ActionResultResponse<DataPage<OrderViewModelGroupByItem>>> GetActiveOrdersGroupedByItem(OrderFilterRequest orderFilterRequest);

        Task<ActionResultResponse<string>> AddSnackOrder(OrderRequest newOrder);

        Task<ActionResultResponse<string>> UpdateSnackOrder(SnackUpdateRequest snackUpdateRequest);

        Task<ActionResultResponse<string>> Insert(OrderRequest newOrder);

        Task<ActionResultResponse<string>> Delete(long id);

        Task<ActionResultResponse<string>> SendReportToHR(long officeId);

        Task<ActionResultResponse<DataPage<OrderViewModel>>> GetEquipmentOrders(OrderFilterRequest orderFilterRequest);

        Task<ActionResultResponse<string>> AddEquipmentOrder(OrderRequest newOrder);

        Task<ActionResultResponse<string>> UpdateEquipmentOrder(UpdateEquipmentOrderRequest updateEquipmentOrderRequest);
    }
}
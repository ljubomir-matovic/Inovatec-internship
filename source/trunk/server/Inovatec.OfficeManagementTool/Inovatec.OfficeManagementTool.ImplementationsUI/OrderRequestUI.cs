using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class OrderRequestUI : IOrderRequestUI
    {
        private readonly IOrderRequestBL _orderRequestBL;
        private readonly IMapper _mapper;

        public OrderRequestUI(IOrderRequestBL orderBL, IMapper mapper)
        {
            _orderRequestBL = orderBL;
            _mapper = mapper;
        }

        public async Task<ActionResultResponse<OrderViewModel?>> GetOrderById(int id)
        {
            return await _orderRequestBL.GetOrderById(id);
        }

        public async Task<ActionResultResponse<DataPage<OrderViewModel>>> GetActiveOrders(OrderFilterRequest orderFilterRequest)
        {
            return await _orderRequestBL.GetActiveOrders(orderFilterRequest);
        }

        public async Task<ActionResultResponse<DataPage<OrderViewModelGroupByItem>>> GetActiveOrdersGroupedByItem(OrderFilterRequest orderFilterRequest)
        {
            return await _orderRequestBL.GetActiveOrdersGroupedByItem(orderFilterRequest);
        }

        public async Task<ActionResultResponse<string>> AddSnackOrder(SnackCreateRequest snackCreateRequest)
        {
            OrderRequest newSnackOrders = _mapper.Map<OrderRequest>(snackCreateRequest);
            newSnackOrders.ItemId = snackCreateRequest.ItemId;
            return await _orderRequestBL.AddSnackOrder(newSnackOrders);
        }

        public async Task<ActionResultResponse<string>> UpdateSnackOrder(SnackUpdateRequest snackUpdateRequest)
        {
            return await _orderRequestBL.UpdateSnackOrder(snackUpdateRequest);
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            return await _orderRequestBL.Delete(id);
        }

        public async Task<ActionResultResponse<string>> AddEquipmentOrder(EquipmentOrderCreateRequest equipmentOrderCreateRequest)
        {
            OrderRequest newOrder = _mapper.Map<OrderRequest>(equipmentOrderCreateRequest);
            return await _orderRequestBL.AddEquipmentOrder(newOrder);
        }

        public async Task<ActionResultResponse<DataPage<OrderViewModel>>> GetEquipmentOrders(OrderFilterRequest orderFilterRequest)
        {
            return await _orderRequestBL.GetEquipmentOrders(orderFilterRequest);
        }

        public async Task<ActionResultResponse<string>> UpdateEquipmentOrder(UpdateEquipmentOrderRequest updateEquipmentOrderRequest)
        {
            return await _orderRequestBL.UpdateEquipmentOrder(updateEquipmentOrderRequest);
        }
    }
}
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class OrderItemUI: IOrderItemUI
    {
        public readonly IOrderItemBL _orderItemBL;

        public OrderItemUI(IOrderItemBL orderItemBL) 
        {
            _orderItemBL = orderItemBL;
        }

        public async Task<DataPage<OrderItemViewModel>> GetOrderItemsForOrder(long orderId, EquipmentFilterRequest filterRequest)
        {
            DataPage<OrderItemViewModel> dataPage = new DataPage<OrderItemViewModel>();

            var result = await _orderItemBL.GetOrderItemsForOrder(orderId, filterRequest);

            dataPage.Data = result.Item1;
            dataPage.TotalRecords = result.Item2;

            return dataPage;
        }

        public async Task<ActionResultResponse<string>> Insert(OrderItemCreateRequest entity)
        {
            return await _orderItemBL.Insert(entity);
        }

        public async Task<ActionResultResponse<string>> Update(OrderItemCreateRequest entity)
        {
            return await _orderItemBL.Update(entity);
        }

        public Task<ActionResultResponse<string>> Delete(long id)
        {
            return _orderItemBL.Delete(id);
        }

        public Task<ActionResultResponse<string>> DeleteMore(List<long> ids)
        {
            return _orderItemBL.DeleteMore(ids);
        }
    }
}
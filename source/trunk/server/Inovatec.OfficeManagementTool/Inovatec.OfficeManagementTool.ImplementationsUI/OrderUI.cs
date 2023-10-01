using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class OrderUI : IOrderUI
    {
        private readonly IOrderBL _orderBL;
        private readonly IMapper _mapper;

        public OrderUI(IOrderBL orderBL, IMapper mapper)
        {
            _orderBL = orderBL;
            _mapper = mapper;
        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await _orderBL.GetOrderById(id);
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            return await _orderBL.Delete(id);
        }

        public async Task<ActionResultResponse<string>> CreateOrderFromCart()
        {
            return await _orderBL.CreateOrderFromCart();
        }

        public async Task<ActionResultResponse<DataPage<Order>>> GetOrders(OrderFilterRequest orderFilterRequest)
        {
            return await _orderBL.GetOrders(orderFilterRequest);
        }

        public async Task<ActionResultResponse<string>> ChangeState(OrderUpdateRequest requestBody)
        {
            return await _orderBL.ChangeState(requestBody);
        }
    }
}

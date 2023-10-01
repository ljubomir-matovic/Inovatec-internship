using Inovatec.OfficeManagementTool.Common.Services.BaseHubService;
using Inovatec.OfficeManagementTool.Common.Services.UserService;
using Inovatec.OfficeManagementTool.Hubs;
using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class OrderBL : IOrderBL
    {
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;
        private readonly INotificationBL _notificationBL;
        private readonly IUserService _userService;
        private readonly IBaseHubService<NotificationHub> _hubService;

        public OrderBL(IUnitOfWorkProvider unitOfWorkProvider, INotificationBL notificationBL, IUserService userService, IBaseHubService<NotificationHub> hubService)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _notificationBL = notificationBL;
            _userService = userService;
            _hubService = hubService;
        }

        public async Task<Order?> GetOrderById(int id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                Order? order = await unitOfWork.OrderDAL.GetById(id);

                if (order == null || order.IsDeleted == true)
                {
                    throw new Exception("OrderCouldNotBeFound");
                }

                var officeId = await unitOfWork.UserDAL.GetOfficeId(_userService.GetUserId() ?? 0);

                if(order.OfficeId != officeId) 
                { 
                    return null;
                }

                return order;
            }
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Order? order = await unitOfWork.OrderDAL.GetById(id);

                if (order == null)
                {
                    throw new Exception("OrderDoesNotExist");
                }

                await unitOfWork.OrderDAL.LogicalDelete(id);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "OrderDeleteSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> CreateOrderFromCart()
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();
            using(var unitOfWork = _unitOfWorkProvider.Begin())
            {
                long officeId = await unitOfWork.UserDAL.GetOfficeId(_userService.GetUserId() ?? 0);
                OutputParameter<int> orderId = new OutputParameter<int>();
                var _ = await unitOfWork.OrderDAL.CreateOrder(officeId,orderId);
                result.ActionSuccess = true;
                result.ActionData = orderId.Value.ToString();
                await _notificationBL.CreateNotificationForNewOrder(orderId.Value);
                await _hubService.SendMessageToGroup(NotificationHub.OfficeGroupPrefix + officeId, "clearedOrders");
            }

            return result;
        }

        public async Task<ActionResultResponse<DataPage<Order>>> GetOrders(OrderFilterRequest orderFilterRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<DataPage<Order>> result = new();

                orderFilterRequest.OfficeId = await unitOfWork.UserDAL.GetOfficeId(_userService.GetUserId() ?? 0);

                (List<Order>, long) pageData = await unitOfWork.OrderDAL.GetOrdersPage(orderFilterRequest);

                DataPage<Order> orderPage = new()
                {
                    Data = pageData.Item1,
                    TotalRecords = pageData.Item2
                };

                result.ActionSuccess = true;
                result.ActionData = orderPage;
                return result;
            }
        }
        public async Task<ActionResultResponse<string>> ChangeState(OrderUpdateRequest requestBody)
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();

            using(var unitOfWork = _unitOfWorkProvider.Begin())
            {
                var order = await unitOfWork.OrderDAL.GetById(requestBody.Id);

                if(order == null)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("");
                    return result;
                }

                OrderState oldState = (OrderState) order.State;

                order.State = (int?)requestBody.State;

                await unitOfWork.OrderDAL.Update(order);
                await unitOfWork.SaveChangesAsync();
                await _notificationBL.CreateNotificationForChangedStateOfOrder(order.OfficeId, order.Id, oldState, (OrderState) order.State);
            }

            return result;
        }
    }
}
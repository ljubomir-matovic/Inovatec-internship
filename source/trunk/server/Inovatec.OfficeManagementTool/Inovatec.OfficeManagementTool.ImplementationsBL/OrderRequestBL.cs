using AutoMapper;
using Inovatec.OfficeManagementTool.Common;
using Inovatec.OfficeManagementTool.Common.Services.BaseHubService;
using Inovatec.OfficeManagementTool.Common.Services.EmailService;
using Inovatec.OfficeManagementTool.Common.Services.UserService;
using Inovatec.OfficeManagementTool.Hubs;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class OrderRequestBL : IOrderRequestBL
    {
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly INotificationBL _notificationBL;
        private readonly IBaseHubService<NotificationHub> _hubService;

        public OrderRequestBL(
            IUnitOfWorkProvider unitOfWorkProvider, 
            IMapper mapper, 
            IEmailService emailService, 
            IUserService userService,
            INotificationBL notificationBL,
            IBaseHubService<NotificationHub> hubService)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _mapper = mapper;
            _emailService = emailService;
            _userService = userService;
            _notificationBL = notificationBL;
            _hubService = hubService;
        }

        public async Task<ActionResultResponse<OrderViewModel?>> GetOrderById(int id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<OrderViewModel?> result = new();

                OrderRequest? order = await unitOfWork.OrderRequestDAL.GetFullOrderById(id);

                if (order == null || order.IsDeleted == true)
                {
                    throw new Exception("OrderCouldNotBeFound");
                }

                result.ActionSuccess = true;
                result.ActionData = _mapper.Map<OrderViewModel?>(order);
                return result;
            }
        }

        public async Task<ActionResultResponse<DataPage<OrderViewModel>>> GetActiveOrders(OrderFilterRequest orderFilterRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<DataPage<OrderViewModel>> result = new();

                (List<OrderRequest>, long) pageData = await unitOfWork.OrderRequestDAL.GetActiveOrderPage(orderFilterRequest);

                DataPage<OrderViewModel> orderPage = new()
                {
                    Data = _mapper.Map<List<OrderViewModel>>(pageData.Item1),
                    TotalRecords = pageData.Item2
                };

                result.ActionSuccess = true;
                result.ActionData = orderPage;
                return result;
            }
        }

        public async Task<ActionResultResponse<DataPage<OrderViewModelGroupByItem>>> GetActiveOrdersGroupedByItem(OrderFilterRequest orderFilterRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<DataPage<OrderViewModelGroupByItem>> result = new();

                (List<OrderViewModelGroupByItem>, long) pageData = await unitOfWork.OrderRequestDAL.GetActiveOrdersGroupedByItem(orderFilterRequest);

                DataPage<OrderViewModelGroupByItem> orderPage = new()
                {
                    Data = pageData.Item1,
                    TotalRecords = pageData.Item2
                };

                result.ActionSuccess = true;
                result.ActionData = orderPage;
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> AddSnackOrder(OrderRequest newOrder)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                long? userId = _userService.GetUserId() ?? throw new Exception("LoggedUserNoIdError");
                newOrder.UserId = (long)userId;

                OrderRequest? existingOrder = (await unitOfWork.OrderRequestDAL.GetBySpecificProperty(
                                                order => order.DateTo == null 
                                                    && order.IsDeleted == false
                                                    && order.ItemId == newOrder.ItemId 
                                                    && order.UserId == newOrder.UserId))
                                                    .FirstOrDefault();
                if(existingOrder == null)
                {
                    await unitOfWork.OrderRequestDAL.Insert(newOrder);
                    await unitOfWork.SaveChangesAsync();
                    OrderRequest? createdOrder = await unitOfWork.OrderRequestDAL.GetOrderRequest(newOrder.Id);
                    if (createdOrder != null)
                    {
                        await _hubService.SendMessageToGroup("OfficeOrders" + newOrder.OfficeId, "addSnackOrder", _mapper.Map<OrderViewModel>(createdOrder), newOrder.Amount, _userService.GetUserId());
                    }
                }
                else
                {
                    existingOrder.Amount += newOrder.Amount;
                    await unitOfWork.SaveChangesAsync();
                    OrderRequest? fullOrder = await unitOfWork.OrderRequestDAL.GetOrderRequest(existingOrder.Id);
                    await _hubService.SendMessageToGroup("OfficeOrders" + newOrder.OfficeId, "addSnackOrder", _mapper.Map<OrderViewModel>(existingOrder), newOrder.Amount, _userService.GetUserId());
                }

                result.ActionSuccess = true;
                result.ActionData = "SnackOrderAddSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> UpdateSnackOrder(SnackUpdateRequest snackUpdateRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                OrderRequest? orderRequest = await unitOfWork.OrderRequestDAL.GetById(snackUpdateRequest.OrderId) ?? throw new Exception("SnackOrderDoesNotExist");

                orderRequest.Amount = snackUpdateRequest.Amount;
                orderRequest.OfficeId = snackUpdateRequest.OfficeId;

                await unitOfWork.OrderRequestDAL.Update(orderRequest);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "SnackOrderUpdateSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Insert(OrderRequest newOrder)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                long? userId = _userService.GetUserId() ?? throw new Exception("LoggedUserNoIdError");
                newOrder.UserId = (long)userId;

                await unitOfWork.OrderRequestDAL.Insert(newOrder);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "ProblemReportAddSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                OrderRequest? fullOrder = await unitOfWork.OrderRequestDAL.GetOrderRequest(id) ?? throw new Exception("OrderDoesNotExist");
                await unitOfWork.OrderRequestDAL.LogicalDelete(id);
                await unitOfWork.SaveChangesAsync();

                if(fullOrder.Description == null)
                {
                    await _hubService.SendMessageToGroup("OfficeOrders" + fullOrder.OfficeId, "deleteSnackOrder", _mapper.Map<OrderViewModel>(fullOrder), _userService.GetUserId());
                }

                result.ActionSuccess = true;
                result.ActionData = "OrderDeleteSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> SendReportToHR(long officeId)
        {
            var result = new ActionResultResponse<string>();

            using(var unitOfWork = _unitOfWorkProvider.Begin())
            {
                var orders = await unitOfWork.OrderRequestDAL.GetSnacksOrderReport(officeId);

                if(orders.Count == 0) 
                {
                    return result;
                }
                
				string body = @"
					<tr>
						<td style=""padding:36px 30px 5px 30px;"">
							<table role=""presentation"" style=""width:100%;border-collapse:collapse;border:0;border-spacing:0;"">
								<tr>
									<td style=""padding:0 0 0 0;color:#153643;"">
										<h1 style=""font-size:24px;margin:0 0 10px 0;font-family:Arial,sans-serif;"">Accept order</h1>
										<p style=""margin:0 0 12px 0;font-size:16px;line-height:24px;font-family:Arial,sans-serif;"">Orders for this week is shown in table below.</p>
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td>
							<div>
							<table id=""order"" style=""margin:auto;"">
								<tr>
									<th>Name</th>
									<th>Amount</th>
								</tr>";

                foreach (var order in orders)
                {
                    body += "<tr>";
                    body += $"<td>{order.Name}</td> <td>{order.Amount}</td>";
                    body += "</tr>";
                }

                body += @"
							</table>
						    </div>
							<div style=""padding-bottom: 20px;padding-top:10px; cursor:pointer;"">
								<div style=""text-decoration:none;margin:auto;""><button style=""margin: auto;cursor:pointer;""><a href=" + ConfigProvider.ClientUrl + "/accept-order" + @" style=""text-decoration:none;color:white;""><span>Accept</span></a></button></div>
							</div>
						</td>
					</tr>";

                var emails = await unitOfWork.UserDAL.GetHREmails(officeId);

                foreach(var email in emails)
                {
                    await _emailService.SendEmail(email, "Snacks order report", string.Empty, body);
                }
            }

            return result;
        }

        public async Task<ActionResultResponse<DataPage<OrderViewModel>>> GetEquipmentOrders(OrderFilterRequest orderFilterRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<DataPage<OrderViewModel>> result = new();

                (List<OrderRequest>, long) pageData = await unitOfWork.OrderRequestDAL.GetEquipmentPage(orderFilterRequest);

                DataPage<OrderViewModel> orderPage = new()
                {
                    Data = _mapper.Map<List<OrderViewModel>>(pageData.Item1),
                    TotalRecords = pageData.Item2
                };

                result.ActionSuccess = true;
                result.ActionData = orderPage;
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> AddEquipmentOrder(OrderRequest newOrderRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                long? userId = _userService.GetUserId() ?? throw new Exception("LoggedUserNoIdError");

                newOrderRequest.UserId = (long)userId;

                await unitOfWork.OrderRequestDAL.Insert(newOrderRequest);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "EquipmentOrderAddSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> UpdateEquipmentOrder(UpdateEquipmentOrderRequest updateEquipmentOrderRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                OrderRequest? orderRequest = await unitOfWork.OrderRequestDAL.GetById(updateEquipmentOrderRequest.Id) ?? throw new Exception("EquipmentOrderDoesNotExist");

                bool changedOrderState = orderRequest.State != updateEquipmentOrderRequest.State;

                orderRequest.Description = updateEquipmentOrderRequest.Description;
                orderRequest.State = updateEquipmentOrderRequest.State;

                await unitOfWork.OrderRequestDAL.Update(orderRequest);
                await unitOfWork.SaveChangesAsync();

                var user = await unitOfWork.UserDAL.GetById(orderRequest.UserId); 

                if (changedOrderState)
                {
                    Comment newComment = new()
                    {
                        OrderId = orderRequest.Id,
                        UserId = _userService.GetUserId() ?? throw new Exception("LoggedUserNoIdError"),
                        OrderState = orderRequest.State,
                        Type = (byte)CommentTypes.StateChange
                    };
                    await unitOfWork.CommentDAL.Insert(newComment);
                    await unitOfWork.SaveChangesAsync();

                    await _hubService.SendMessageToAll("newCommentForOrderRequest" + updateEquipmentOrderRequest.Id, _mapper.Map<CommentViewModel?>(await unitOfWork.CommentDAL.GetComment(newComment.Id)));
                }

                result.ActionSuccess = true;
                result.ActionData = "EquipmentOrderUpdateSuccess";
                return result;
            }
        }
    }
}
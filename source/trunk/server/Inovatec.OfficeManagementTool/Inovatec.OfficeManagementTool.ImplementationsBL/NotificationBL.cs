using AutoMapper;
using Inovatec.OfficeManagementTool.Common;
using Inovatec.OfficeManagementTool.Common.Services.BaseHubService;
using Inovatec.OfficeManagementTool.Common.Services.EmailService;
using Inovatec.OfficeManagementTool.Common.Services.UserService;
using Inovatec.OfficeManagementTool.Hubs;
using Inovatec.OfficeManagementTool.ImplementationsDAL;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class NotificationBL : INotificationBL
    {
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IBaseHubService<NotificationHub> _hubService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public NotificationBL(
            IUnitOfWorkProvider unitOfWorkProvider, 
            IBaseHubService<NotificationHub> hubService, 
            IEmailService emailService,
            IUserService userService) 
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _hubService = hubService;
            _emailService = emailService;
            _userService = userService;
        }

        private async Task CreateNotification(Notification notification)
        {
            using(var unitOfWork = _unitOfWorkProvider.Begin())
            {
                await unitOfWork.NotificationDAL.Insert(notification);
                await unitOfWork.SaveChangesAsync();
            }
        }

        private async Task CreateNotificationForHRs(NotificationCreateRequest createRequest)
        {
            using(var unitOfWork = _unitOfWorkProvider.Begin())
            {
                var hrs = await unitOfWork.UserDAL.GetHRsForOffice(await unitOfWork.UserDAL.GetOfficeId(_userService.GetUserId() ?? 0));

                if (!createRequest.IncludeSender)
                {
                    hrs = hrs.Where(x => x.Id != _userService.GetUserId()).ToList();
                }

                foreach (var hr in hrs)
                {
                    Notification notification = new Notification
                    {
                        Data = createRequest.Data,
                        IsRead = false,
                        Description = createRequest.Description,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        IsDeleted = false,
                        UserId = hr.Id
                    };
                    try
                    {
                        await CreateNotification(notification);
                        await _hubService.SendMessageToGroup(BaseHub.CurrentUserPrefix + hr.Id, "newNotification", notification.Id, createRequest.Data, createRequest.Description, notification.DateCreated);
                        await _emailService.SendEmail(hr.Email, createRequest.Subject, createRequest.Style, createRequest.Body);
                    }
                    catch { }
                }
            }
        }

        public async Task<DataPage<NotificationViewModel>> GetNotifications(NotificationFilterRequest filterRequest)
        {
            DataPage<NotificationViewModel> page = new DataPage<NotificationViewModel>();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                filterRequest.UserId = _userService.GetUserId();
                var result = await unitOfWork.NotificationDAL.GetNotifications(filterRequest);

                page.Data = result.Item1;
                page.TotalRecords = result.Item2;
                page.More = result.Item3;

                return page;
            }
        }

        private async Task CreateNotificationForUser(NotificationCreateRequest createRequest)
        {
            Notification notification = new Notification
            {
                    Data = createRequest.Data,
                    IsRead = false,
                    Description = createRequest.Description,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    IsDeleted = false,
                    UserId = createRequest.UserId
             };
            try
            {
               await CreateNotification(notification);
               await _hubService.SendMessageToGroup(BaseHub.CurrentUserPrefix + createRequest.UserId, "newNotification", notification.Id, createRequest.Data, createRequest.Description, notification.DateCreated);
               await _emailService.SendEmail(createRequest.Email, createRequest.Subject, createRequest.Style, createRequest.Body);
            }
            catch { }
        }

        public async Task CreateNotificationForNewOrder(long orderId)
        {
            string subject = "New order created";
            string body = @"<p style='padding:20px;font-size: 16px;'>You can see new order on <a href=" + 
                ConfigProvider.ClientUrl + "/order/" + orderId + @">this url<a/>.</p>";
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(new { type = NotificationType.Url, url = "/order/" + orderId });
            await CreateNotificationForHRs(new NotificationCreateRequest 
            {
                Data = data, 
                Description = "Notification.NewOrderCreated", 
                Subject = subject, 
                Body = body 
            });
        }

        public async Task CreateNotificationForChangedStateOfOrder(long officeId, long orderId, OrderState oldState, OrderState newState)
        {
            string subject = "Order #" + orderId + " state changed";
            string body = @"<p style='padding:20px;font-size: 16px;'>HR manager " + _userService.GetFullName() + @" changed state
            of order <a href=" +
                ConfigProvider.ClientUrl + "/order/" + orderId + @">#" + orderId + @"<a/> from " + Enum.GetName(typeof(OrderState), oldState) + @"
            to " + Enum.GetName(typeof(OrderState), newState) + @".</p>";
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(new { type = NotificationType.Url, url = "/order/" + orderId });
            await CreateNotificationForHRs(new NotificationCreateRequest
            {
                IncludeSender = false,
                Data = data,
                Description = "Notification.OrderStateChanged",
                Subject = subject,
                Body = body
            });
            await _hubService.SendMessageToGroup(NotificationHub.HRGroupPrefix + officeId, "changedStateOfOrder" + orderId, newState);
        }

        public async Task<int> GetUnreadNotificationNumber()
        {
            long id = _userService.GetUserId() ?? 0;

            using(var unitOfWork = _unitOfWorkProvider.Begin())
            {
                return await unitOfWork.NotificationDAL.GetUnreadNotificationNumber(id);
            }
        }

        public async Task<ActionResultResponse> MarkNotificationAsRead(long id)
        {
            var result = new ActionResultResponse();

            using(var unitOfWork = _unitOfWorkProvider.Begin())
            {
                var notification = await unitOfWork.NotificationDAL.GetById(id);

                if(notification == null) 
                {
                    result.ActionSuccess = false;
                    return result;
                }

                notification.IsRead = true;

                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                return result;
            }
        }

        public async Task<Notification?> GetById(long id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                return await unitOfWork.NotificationDAL.GetById(id);
            }
        }

        public async Task MarkAllAsRead(long userId)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                var notifications = await unitOfWork.NotificationDAL.GetBySpecificProperty(x => x.IsRead == false && x.UserId == userId);
                notifications.ForEach(notification =>  notification.IsRead = true);

                await unitOfWork.SaveChangesAsync();
            }
        }

        public async Task ChangedEquipmentOrder(UpdateEquipmentOrderRequest updateEquipmentOrderRequest, long ownerId, string ownerEmail)
        {
            string subject = "Equipment order #" + updateEquipmentOrderRequest.Id + " changed";
            string body = @"<p style='padding:20px;font-size: 16px;'>" + _userService.GetFullName() + @" changed equipment
            order request <a href=" +
                ConfigProvider.ClientUrl + "/order/" + updateEquipmentOrderRequest.Id + @">#" + updateEquipmentOrderRequest.Id + @"<a/>";
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(new { type = NotificationType.Url, url = "/order/" + updateEquipmentOrderRequest.Id });
            await CreateNotificationForUser(new NotificationCreateRequest {
                UserId = ownerId,
                Email = ownerEmail,
                IncludeSender = false,
                Data = data, 
                Description = "Notification.OrderStateChanged", 
                Subject = subject, 
                Body = body
            });
        }
    }
}

using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class NotificationDAL : BaseDAL<OfficeManagementTool_IS2023Context, Notification>, INotificationDAL
    {
        public async Task<(List<NotificationViewModel>, int, bool)> GetNotifications(NotificationFilterRequest filterRequest)
        {
            var query = Table.Where(notification => notification.IsDeleted == false
                && notification.IsRead == filterRequest.IsRead
                && (filterRequest.UpperBound == -1 || notification.Id < filterRequest.UpperBound)
                && notification.UserId == filterRequest.UserId
            )
            .Select(notification => new NotificationViewModel
            {
                Id = notification.Id,
                Data = notification.Data,
                IsRead = (bool)notification.IsRead,
                DateCreated = notification.DateCreated,
                Description = notification.Description
            })
            .OrderByDescending(notification => notification.DateCreated).AsQueryable();

            bool more = true;
            var lengthBefore = query.Count();

            if(filterRequest.PageSize > 0)
            {
                int pageNumber = filterRequest.PageNumber ?? 0;
                int pageSize = filterRequest.PageSize ?? 0;

                if(lengthBefore < pageSize || (pageNumber > 0 && Math.Ceiling((decimal)(lengthBefore / pageSize)) == pageNumber)) 
                { 
                    more = false;
                }

                if (filterRequest.PageNumber > 0) 
                {
                    query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }
                else
                {
                    query = query.Take(pageSize);
                }
            }
            else if (lengthBefore == 0)
            {
                more = false;
            }

            return (await query.ToListAsync(), lengthBefore, more);
        }

        public async Task<int> GetUnreadNotificationNumber(long id)
        {
            return await Table.Where(notification => notification.IsDeleted == false 
                && notification.IsRead == false
                && notification.UserId == id).CountAsync();
        }
    }
}

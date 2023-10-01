using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class OrderAttachmentDAL : BaseDAL<OfficeManagementTool_IS2023Context, OrderAttachment>, IOrderAttachmentDAL
    {
        public async Task<List<OrderAttachmentViewModel>> GetAttachmentsForOrder(long orderId)
        {
            return await Table
                .Where(attachment => attachment.OrderId == orderId && !(attachment.IsDeleted ?? true))
                .Select(attachment => new OrderAttachmentViewModel
                {
                    Id = attachment.Id,
                    Name = attachment.Name,
                })
                .ToListAsync();
        }
    }
}
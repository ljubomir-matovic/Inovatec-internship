using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface IOrderAttachmentDAL : IBaseDAL<OrderAttachment>
    {
        Task<List<OrderAttachmentViewModel>> GetAttachmentsForOrder(long orderId);
    }
}
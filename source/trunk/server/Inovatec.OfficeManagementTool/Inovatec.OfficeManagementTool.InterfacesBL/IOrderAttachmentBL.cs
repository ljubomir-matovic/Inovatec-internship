using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface IOrderAttachmentBL
    {
        Task<OrderAttachment?> GetAttachmentById(long id);
        Task<List<OrderAttachmentViewModel>> GetAttachmentsForOrder(long orderId);
        Task<ActionResultResponse> UploadFile(FileUploadRequest requestBody);
        Task<ActionResultResponse> Delete(long id);
    }
}
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesUI
{
    public interface IOrderAttachmentUI
    {
        Task<OrderAttachment?> GetAttachmentById(long id);
        Task<List<OrderAttachmentViewModel>> GetAttachmentsForOrder(long orderId);
        Task<ActionResultResponse> UploadFile(FileUploadRequest requestBody);
        Task<ActionResultResponse> Delete(long id);
    }
}
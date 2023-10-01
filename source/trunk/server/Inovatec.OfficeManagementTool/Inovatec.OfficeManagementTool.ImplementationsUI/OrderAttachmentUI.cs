using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class OrderAttachmentUI : IOrderAttachmentUI
    {
        private readonly IOrderAttachmentBL _orderAttachmentBL;

        public OrderAttachmentUI(IOrderAttachmentBL orderAttachmentBL) 
        {
            _orderAttachmentBL = orderAttachmentBL;
        }

        public async Task<OrderAttachment?> GetAttachmentById(long id)
        {
            return await _orderAttachmentBL.GetAttachmentById(id);
        }

        public async Task<List<OrderAttachmentViewModel>> GetAttachmentsForOrder(long orderId)
        {
            return await _orderAttachmentBL.GetAttachmentsForOrder(orderId);
        }

        public async Task<ActionResultResponse> UploadFile(FileUploadRequest requestBody)
        {
            return await _orderAttachmentBL.UploadFile(requestBody);
        }

        public async Task<ActionResultResponse> Delete(long id)
        {
            return await _orderAttachmentBL.Delete(id);
        }
    }
}
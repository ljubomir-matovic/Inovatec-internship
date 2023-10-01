using Inovatec.OfficeManagementTool.Common;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using System.Net.Mime;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class OrderAttachmentBL : IOrderAttachmentBL
    {
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;

        public OrderAttachmentBL(IUnitOfWorkProvider unitOfWorkProvider) 
        { 
            _unitOfWorkProvider = unitOfWorkProvider;
        }

        public async Task<OrderAttachment?> GetAttachmentById(long id)
        {
            using(var unitOfWork = _unitOfWorkProvider.Begin())
            {
                return await unitOfWork.OrderAttachmentDAL.GetById(id);
            }
        }

        public async Task<List<OrderAttachmentViewModel>> GetAttachmentsForOrder(long orderId)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                return await unitOfWork.OrderAttachmentDAL.GetAttachmentsForOrder(orderId);
            }
        }

        public async Task<ActionResultResponse> UploadFile(FileUploadRequest requestBody)
        {
            ActionResultResponse result = new ActionResultResponse();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                using (var transaction = unitOfWork.BeginTransaction())
                {
                    try
                    {
                        foreach (var file in requestBody.Files)
                        {
                            if (!ConfigProvider.AllowedContentTypes.Contains(file.ContentType))
                            {
                                continue;
                            }

                            var attachment = new OrderAttachment
                            {
                                Name = Path.GetFileName(file.FileName),
                                ContentType = file.ContentType,
                                OrderId = requestBody.OrderId,
                                IsDeleted = false,
                                DateCreated = DateTime.Now,
                                DateModified = DateTime.Now,
                            };

                            long fileLength = file.Length;

                            attachment.Content = new byte[fileLength];

                            using (var stream = file.OpenReadStream())
                            {
                                await stream.ReadAsync(attachment.Content, 0, (int)fileLength);

                                stream.Close();
                            }

                            await unitOfWork.OrderAttachmentDAL.Insert(attachment);
                        }

                        await unitOfWork.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                    }
                }
            }

            result.ActionSuccess = true;
            result.ActionData = "Success";

            return result;
        }

        public async Task<ActionResultResponse> Delete(long id)
        {
            ActionResultResponse result = new ActionResultResponse();

            using(var unitOfWork = _unitOfWorkProvider.Begin())
            {
                result.ActionData = await unitOfWork.OrderAttachmentDAL.LogicalDelete(id);
                await unitOfWork.SaveChangesAsync();
            }

            result.ActionSuccess = true;
            return result;
        }
    }
}
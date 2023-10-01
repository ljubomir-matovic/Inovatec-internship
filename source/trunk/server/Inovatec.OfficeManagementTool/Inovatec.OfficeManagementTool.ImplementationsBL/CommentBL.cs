using AutoMapper;
using Inovatec.OfficeManagementTool.Common.Services.BaseHubService;
using Inovatec.OfficeManagementTool.Hubs;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class CommentBL : ICommentBL
    {
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IMapper _mapper;
        private readonly IBaseHubService<NotificationHub> _hubService;

        public CommentBL(IUnitOfWorkProvider unitOfWorkProvider, IMapper mapper, IBaseHubService<NotificationHub> hubService)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _mapper = mapper;
            _hubService = hubService;
        }

        public async Task<ActionResultResponse<CommentViewModel?>> GetCommentById(int id)
        {
            using(var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<CommentViewModel?> result = new();

                Comment? comment = await unitOfWork.CommentDAL.GetComment(id);

                if (comment == null || comment.IsDeleted == true)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("CommentCouldNotBeFound");
                    return result;
                }

                result.ActionSuccess = true;
                result.ActionData = _mapper.Map<CommentViewModel?>(comment);
                return result;
            }
        }

        public async Task<ActionResultResponse<DataPage<CommentViewModel>>> GetComments(CommentFilterRequest commentFilterRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<DataPage<CommentViewModel>> result = new();

                (List<Comment>, long) pageData = await unitOfWork.CommentDAL.GetCommentPage(commentFilterRequest);
                
                DataPage<CommentViewModel> commentPage = new()
                {
                    Data = _mapper.Map<List<CommentViewModel>>(pageData.Item1),
                    TotalRecords = pageData.Item2
                };

                result.ActionSuccess = true;
                result.ActionData = commentPage;
                return result;
            }
        }

        public async Task<ActionResultResponse<CommentViewModel>> Insert(Comment newComment)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<CommentViewModel> result = new();

                var resultComment = await unitOfWork.CommentDAL.InsertAndReturn(newComment);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = _mapper.Map<CommentViewModel>(resultComment);

                string eventType;

                if (newComment.ReportId != null)
                {
                    eventType = "newCommentForReport" + newComment.ReportId;
                }
                else
                {
                    eventType = "newCommentForOrderRequest" + newComment.OrderId;
                }

                var comment = _mapper.Map<CommentViewModel?>(await unitOfWork.CommentDAL.GetComment(newComment.Id));
                await _hubService.SendMessageToAll(eventType, comment);
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Update(UpdateCommentRequest updatedComment)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Comment? comment = await unitOfWork.CommentDAL.GetById(updatedComment.Id);

                if (comment == null)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("CommentDoesNotExist");
                    return result;
                }

                comment.Text = updatedComment.Text;

                await unitOfWork.CommentDAL.Update(comment);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "CommentUpdateSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Comment? comment = await unitOfWork.CommentDAL.GetById(id);

                if (comment == null)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("CommentDoesNotExist");
                    return result;
                }

                await unitOfWork.CommentDAL.LogicalDelete(id);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "CommentDeleteSuccess";
                return result;
            }
        }
    }
}
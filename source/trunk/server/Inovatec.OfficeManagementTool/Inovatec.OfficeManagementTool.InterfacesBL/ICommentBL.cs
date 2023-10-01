using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface ICommentBL
    {
        Task<ActionResultResponse<CommentViewModel?>> GetCommentById(int id);
        Task<ActionResultResponse<DataPage<CommentViewModel>>> GetComments(CommentFilterRequest commentFilterRequest);
        Task<ActionResultResponse<CommentViewModel>> Insert(Comment newComment);
        Task<ActionResultResponse<string>> Update(UpdateCommentRequest commentViewModel);
        Task<ActionResultResponse<string>> Delete(long id);
    }
}
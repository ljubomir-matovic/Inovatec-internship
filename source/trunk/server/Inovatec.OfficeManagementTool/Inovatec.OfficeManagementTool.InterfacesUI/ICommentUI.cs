using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesUI
{
    public interface ICommentUI
    {
        Task<ActionResultResponse<CommentViewModel?>> GetCommentById(int id);
        Task<ActionResultResponse<DataPage<CommentViewModel>>> GetComments(CommentFilterRequest commentFilter);
        Task<ActionResultResponse<CommentViewModel>> Insert(CommentCreateRequest commentCreateRequest);
        Task<ActionResultResponse<string>> Update(UpdateCommentRequest updatedComment);
        Task<ActionResultResponse<string>> Delete(long id);
    }
}
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface ICommentDAL : IBaseDAL<Comment>
    {
        Task<(List<Comment>, long)> GetCommentPage(CommentFilterRequest commentFilterRequest);
        Task<Comment> GetComment(long id);
        Task<Comment> InsertAndReturn(Comment entityToInsert);
    }
}

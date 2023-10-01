using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class CommentDAL : BaseDAL<OfficeManagementTool_IS2023Context, Comment>, ICommentDAL
    {
        public async Task<(List<Comment>, long)> GetCommentPage(CommentFilterRequest commentFilterRequest)
        {
            var query = Table
                .Where(comment => comment.IsDeleted == false)
                .Include(comment => comment.User)
                .AsQueryable();

            if(commentFilterRequest.GetOrderComments)
            {
                query = query.Where(comment => comment.OrderId != null);
            }

            if (commentFilterRequest.GetReportComments)
            {
                query = query.Where(comment => comment.ReportId != null);
            }

            if(commentFilterRequest.Orders.Count > 0)
            {
                query = query.Where(comment => commentFilterRequest.Orders.Contains((long)comment.OrderId));
            }

            if (commentFilterRequest.Reports.Count > 0)
            {
                query = query.Where(comment => commentFilterRequest.Reports.Contains((long)comment.ReportId));
            }

            if (commentFilterRequest.Users.Count > 0)
            {
                query = query.Where(comment => commentFilterRequest.Users.Contains((long)comment.UserId));
            }

            long total = query.Count();

            if (commentFilterRequest.SortOrder > 0)
            {
                query = query.OrderBy(comment => comment.DateCreated).AsQueryable();
            }
            else
            {
                query = query.OrderByDescending(comment => comment.DateCreated).AsQueryable();
            }

            query = query.Skip(commentFilterRequest.CommentsSkipCount).Take(commentFilterRequest.CommentsBatchSize);

            return (await query.ToListAsync(), total);
        }

        public async Task<Comment?> GetComment(long id)
        {
            return await Table.Where(comment => comment.Id == id && comment.IsDeleted == false).Include(comment => comment.User).FirstOrDefaultAsync();
        }

        public virtual async Task<Comment> InsertAndReturn(Comment entityToInsert)
        {
            await Table.AddAsync(entityToInsert);
            return entityToInsert;
        }
    }
}
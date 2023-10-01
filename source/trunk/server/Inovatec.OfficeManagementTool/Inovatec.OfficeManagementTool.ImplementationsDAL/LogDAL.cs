using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class LogDAL : BaseDAL<OfficeManagementTool_IS2023Context, Log>, ILogDAL
    {
        public async Task<(List<Log>, long)> GetLogs(LogsFilterRequest orderFilterRequest)
        {
            var query = Table.AsQueryable();

            if (!string.IsNullOrEmpty(orderFilterRequest.Message))
            {
                query = query
                    .Where(order => order.Message.ToLower().Contains(orderFilterRequest.Message));
            }

            if (!string.IsNullOrEmpty(orderFilterRequest.Exception))
            {
                query = query
                    .Where(order => order.Exception.ToLower().Contains(orderFilterRequest.Exception));
            }

            if (!string.IsNullOrEmpty(orderFilterRequest.User))
            {
                query = query
                    .Where(order => order.User.ToLower().Contains(orderFilterRequest.User));
            }

            long total = query.Count();

            SetOrderBy(orderFilterRequest.SortField, orderFilterRequest.SortOrder, ref query);

            if (orderFilterRequest.PageNumber > 0 && orderFilterRequest.PageSize > 0)
            {
                query = query
                    .Skip((orderFilterRequest.PageNumber - 1) * orderFilterRequest.PageSize)
                    .Take(orderFilterRequest.PageSize);
            }

            return (await query.ToListAsync(), total);
        }

        private void SetOrderBy(string? sortField, int sortOrder, ref IQueryable<Log> query)
        {
            Expression<Func<Log, object>> expression = sortField switch
            {
                "message" => (log => log.Message),
                "exception" => (log => log.Exception),
                "user" => (log => log.User),
                "date" => (log => log.TimeStamp),
                _ => (log => log.Id)
            };

            if (sortOrder > 0)
            {
                query = query
                    .OrderBy(expression)
                    .AsQueryable();
            }
            else
            {
                query = query
                    .OrderByDescending(expression)
                    .AsQueryable();
            }
        }
    }
}

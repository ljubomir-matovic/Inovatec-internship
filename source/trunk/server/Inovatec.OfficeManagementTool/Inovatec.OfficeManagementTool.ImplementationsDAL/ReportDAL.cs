using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class ReportDAL : BaseDAL<OfficeManagementTool_IS2023Context, Report>, IReportDAL
    {
        public async Task<(List<Report>, long)> GetReportPage(ReportFilterRequest reportFilterRequest)
        {
            var query = Table
                .Where( report => report.IsDeleted == false )
                .Include(report => report.User)
                .Include(report => report.Office)
                .Include(report => report.Equipment)
                .ThenInclude(report => report.Item)
                .AsQueryable();

            if (!string.IsNullOrEmpty(reportFilterRequest.Description))
            {
                query = query
                    .Where(report => report.Description.ToLower().Contains(reportFilterRequest.Description));
            }

            if (!string.IsNullOrEmpty(reportFilterRequest.Name))
            {
                query = query
                    .Where(report => $"{report.User.FirstName.ToLower()} {report.User.LastName.ToLower()}".Contains(reportFilterRequest.Name));
            }

            if (reportFilterRequest.Categories != null && reportFilterRequest.Categories.Count > 0)
            {
                query = query
                    .Where(report => reportFilterRequest.Categories.Contains((long)report.Category));
            }

            if (reportFilterRequest.Users != null && reportFilterRequest.Users.Count > 0)
            {
                query = query
                    .Where(report => reportFilterRequest.Users.Contains(report.UserId));
            }

            if (reportFilterRequest.States != null && reportFilterRequest.States.Count > 0)
            {
                query = query
                    .Where(report => reportFilterRequest.States.Contains((byte)report.State));
            }

            if (reportFilterRequest.Offices != null && reportFilterRequest.Offices.Count > 0)
            {
                query = query
                    .Where(report => reportFilterRequest.Offices.Contains((byte)report.OfficeId));
            }

            long total = query.Count();

            SetOrderBy(reportFilterRequest.SortField, reportFilterRequest.SortOrder, ref query);

            if (reportFilterRequest.PageNumber > 0 && reportFilterRequest.PageSize > 0)
            {
                query = query
                    .Skip((reportFilterRequest.PageNumber - 1) * reportFilterRequest.PageSize)
                    .Take(reportFilterRequest.PageSize);
            }

            return (await query.ToListAsync(), total);
        }

        private void SetOrderBy(string? sortField, int sortOrder, ref IQueryable<Report> query)
        {
            Expression<Func<Report, object>> expression = sortField switch
            {
                "itemName" => (report => report.Equipment.Item.Name),
                "description" => (report => report.Description),
                "dateCreated" => (report => report.DateCreated),
                "name" => (report => report.User.FirstName),
                "office" => (report => report.Office.Name),
                _ => (report => report.Id)
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

        public async Task<Report?> GetFullReportById(long id)
        {
            return await Table
                .Include(order => order.User)
                .Include(order => order.Equipment)
                .Where(order => order.Id == id && order.IsDeleted == false)
                .FirstOrDefaultAsync();
        }
    }
}
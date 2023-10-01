using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class ReportScheduleDAL : BaseDAL<OfficeManagementTool_IS2023Context, ReportSchedule>, IReportScheduleDAL
    {
        public async Task<(List<ReportSchedule>, long)> GetReportSchedulePage(ReportScheduleFilterRequest reportScheduleFilterRequest)
        {
            var query = Table
                .Where(reportSchedule => reportSchedule.IsDeleted == false)
                .Include(reportSchedule => reportSchedule.Office)
                .AsQueryable();

            if (reportScheduleFilterRequest.Offices != null && reportScheduleFilterRequest.Offices.Count > 0)
            {
                query = query
                    .Where(reportSchedule => reportScheduleFilterRequest.Offices.Contains((byte)reportSchedule.OfficeId));
            }

            switch (reportScheduleFilterRequest.State)
            {
                case 0:
                    query = query
                        .Where(reportSchedule => reportSchedule.IsActive == false);
                    break;
                case 1:
                    query = query
                        .Where(reportSchedule => reportSchedule.IsActive == true);
                    break;
                default:
                    break;
            }

            long total = query.Count();

            SetOrderBy(reportScheduleFilterRequest.SortField, reportScheduleFilterRequest.SortOrder, ref query);

            if (reportScheduleFilterRequest.PageNumber > 0 && reportScheduleFilterRequest.PageSize > 0)
            {
                query = query
                    .Skip((reportScheduleFilterRequest.PageNumber - 1) * reportScheduleFilterRequest.PageSize)
                    .Take(reportScheduleFilterRequest.PageSize);
            }

            return (await query.ToListAsync(), total);
        }

        private void SetOrderBy(string sortField, int sortOrder, ref IQueryable<ReportSchedule> query)
        {
            Expression<Func<ReportSchedule, object>> expression = sortField switch
            {
                "office" => (reportSchedule => reportSchedule.Office.Name),
                "scheduleDate" => (reportSchedule => reportSchedule.ScheduleDate),
                "dateCreated" => (reportSchedule => reportSchedule.DateCreated),
                _ => (reportSchedule => reportSchedule.Id)
            };

            if(sortOrder > 0)
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

        public async Task<List<ReportSchedule>> GetTodayReportSchedules()
        {
            var query = Table
                .Where(reportSchedule =>
                    reportSchedule.IsDeleted == false &&
                    reportSchedule.IsActive == true &&
                    reportSchedule.ScheduleDate.Day == DateTime.UtcNow.Day &&
                    reportSchedule.ScheduleDate.Month == DateTime.UtcNow.Month &&
                    reportSchedule.ScheduleDate.Year == DateTime.UtcNow.Year
                );
            return await query.ToListAsync();
        }
    }
}
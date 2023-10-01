using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface IReportScheduleDAL : IBaseDAL<ReportSchedule>
    {
        Task<(List<ReportSchedule>, long)> GetReportSchedulePage(ReportScheduleFilterRequest reportScheduleFilterRequest);
        Task<List<ReportSchedule>> GetTodayReportSchedules();
    }
}
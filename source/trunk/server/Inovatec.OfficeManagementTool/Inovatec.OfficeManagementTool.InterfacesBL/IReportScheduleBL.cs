using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface IReportScheduleBL
    {
        Task<DataPage<ReportScheduleViewModel>> GetReportSchedulePage(ReportScheduleFilterRequest reportScheduleFilterRequest);
        Task<ReportScheduleViewModel?> GetReportScheduleById(int id);
        Task<List<ReportSchedule>> GetTodayReportSchedules();
        Task<ActionResultResponse<string>> Insert(ReportSchedule newReportSchedule);
        Task<ActionResultResponse<string>> Update(UpdateReportScheduleRequest updatedReportSchedule);
        Task<ActionResultResponse<string>> Delete(long id);
    }
}

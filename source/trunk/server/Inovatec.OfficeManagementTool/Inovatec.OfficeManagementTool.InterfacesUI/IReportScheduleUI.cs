using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesUI
{
    public interface IReportScheduleUI
    {
        Task<ReportScheduleViewModel?> GetReportScheduleById(int id);
        Task<DataPage<ReportScheduleViewModel>> GetReportSchedulePage(ReportScheduleFilterRequest reportScheduleFilterRequest);
        Task<ActionResultResponse<string>> Insert(ReportScheduleCreateRequest newReportSchedule);
        Task<ActionResultResponse<string>> Update(UpdateReportScheduleRequest updatedReportSchedule);
        Task<ActionResultResponse<string>> Delete(long id);
    }
}
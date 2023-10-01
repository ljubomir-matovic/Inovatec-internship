using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesUI
{
    public interface IReportUI
    {
        Task<ActionResultResponse<ReportViewModel?>> GetReportById(int id);
        Task<ActionResultResponse<DataPage<ReportViewModel>>> GetReports(ReportFilterRequest reportFilterRequest);
        Task<ActionResultResponse<string>> AddReport(ReportCreateRequest newReport);
        Task<ActionResultResponse<string>> UpdateReport(UpdateReportRequest updateReportRequest);
        Task<ActionResultResponse<string>> DeleteReport(long id);
    }
}
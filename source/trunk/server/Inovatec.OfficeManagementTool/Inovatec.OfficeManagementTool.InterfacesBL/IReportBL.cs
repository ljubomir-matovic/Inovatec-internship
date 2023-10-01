using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface IReportBL
    {
        Task<ActionResultResponse<ReportViewModel?>> GetReportById(int id);
        Task<ActionResultResponse<DataPage<ReportViewModel>>> GetReports(ReportFilterRequest reportFilterRequest);
        Task<ActionResultResponse<string>> Insert(Report newOrder);
        Task<ActionResultResponse<string>> Update(UpdateReportRequest updateReportRequest);
        Task<ActionResultResponse<string>> Delete(long id);
    }
}
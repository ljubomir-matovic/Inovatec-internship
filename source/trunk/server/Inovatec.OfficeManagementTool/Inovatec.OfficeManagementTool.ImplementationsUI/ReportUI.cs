using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class ReportUI : IReportUI
    {
        private readonly IReportBL _reportBL;
        private readonly IMapper _mapper;

        public ReportUI(IReportBL reportBL, IMapper mapper)
        {
            _reportBL = reportBL;
            _mapper = mapper;
        }

        public async Task<ActionResultResponse<string>> AddReport(ReportCreateRequest newReport)
        {
            Report report = _mapper.Map<Report>(newReport);
            return await _reportBL.Insert(report);
        }

        public async Task<ActionResultResponse<string>> DeleteReport(long id)
        {
            return await _reportBL.Delete(id);
        }

        public async Task<ActionResultResponse<ReportViewModel?>> GetReportById(int id)
        {
            return await _reportBL.GetReportById(id);
        }

        public async Task<ActionResultResponse<DataPage<ReportViewModel>>> GetReports(ReportFilterRequest reportFilterRequest)
        {
            return await _reportBL.GetReports(reportFilterRequest);
        }

        public async Task<ActionResultResponse<string>> UpdateReport(UpdateReportRequest updateReportRequest)
        {
            return await _reportBL.Update(updateReportRequest);
        }
    }
}
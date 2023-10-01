using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class ReportScheduleUI : IReportScheduleUI
    {
        private readonly IReportScheduleBL _reportScheduleBL;
        private readonly IMapper _mapper;

        public ReportScheduleUI(IReportScheduleBL reportScheduleBL, IMapper mapper) 
        {
            _reportScheduleBL = reportScheduleBL;
            _mapper = mapper;
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            return await _reportScheduleBL.Delete(id);
        }

        public async Task<ReportScheduleViewModel?> GetReportScheduleById(int id)
        {
            return await _reportScheduleBL.GetReportScheduleById(id);
        }

        public async Task<DataPage<ReportScheduleViewModel>> GetReportSchedulePage(ReportScheduleFilterRequest filterRequest)
        {
            return await _reportScheduleBL.GetReportSchedulePage(filterRequest);
        }

        public async Task<ActionResultResponse<string>> Insert(ReportScheduleCreateRequest reportScheduleCreateRequest)
        {
            ReportSchedule newReportSchedule = _mapper.Map<ReportSchedule>(reportScheduleCreateRequest);
            return await _reportScheduleBL.Insert(newReportSchedule);
        }

        public async Task<ActionResultResponse<string>> Update(UpdateReportScheduleRequest updateReportScheduleRequest)
        {
            return await _reportScheduleBL.Update(updateReportScheduleRequest);
        }
    }
}

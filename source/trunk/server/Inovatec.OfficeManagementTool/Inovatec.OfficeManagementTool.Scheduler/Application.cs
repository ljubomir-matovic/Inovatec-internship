using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.Scheduler
{
    public interface IApplication
    {
        Task Run();
    }

    public class Application : IApplication
    {
        private readonly IOrderRequestBL _orderRequestBL;
        private readonly IReportScheduleBL _reportScheduleBL;
        private readonly IMapper _mapper;

        public Application(IOrderRequestBL orderRequestBL, IReportScheduleBL reportScheduleBL, IMapper mapper)
        {
            _orderRequestBL = orderRequestBL;
            _reportScheduleBL = reportScheduleBL;
            _mapper = mapper;
        }

        public async Task Run() 
        {
            List<ReportSchedule> upcomingReportSchedule = await _reportScheduleBL.GetTodayReportSchedules();

            foreach(ReportSchedule schedule in upcomingReportSchedule)
            {
                await _orderRequestBL.SendReportToHR(schedule.OfficeId);
                UpdateReportScheduleRequest updatedReportSchedule = _mapper.Map<UpdateReportScheduleRequest>(schedule);
                updatedReportSchedule.IsActive = false;
                await _reportScheduleBL.Update(updatedReportSchedule);
            }
        }
    }
}
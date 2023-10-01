using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class ReportScheduleBL : IReportScheduleBL
    {
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IMapper _mapper;

        public ReportScheduleBL(IUnitOfWorkProvider unitOfWorkProvider, IMapper mapper)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _mapper = mapper;
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                ReportSchedule? reportSchedule = await unitOfWork.ReportScheduleDAL.GetById(id);

                if (reportSchedule == null || reportSchedule.IsDeleted == true)
                {
                    result.Errors.Add("ReportScheduleDoesNotExist");
                    result.ActionSuccess = false;
                    return result;
                }

                await unitOfWork.ReportScheduleDAL.LogicalDelete(id);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "ReportScheduleDeleteSuccess";
                return result;
            }
        }

        public async Task<ReportScheduleViewModel?> GetReportScheduleById(int id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<ReportScheduleViewModel?> result = new();

                ReportSchedule? reportSchedule = await unitOfWork.ReportScheduleDAL.GetById(id);

                if (reportSchedule == null || reportSchedule.IsDeleted == true)
                {
                    return null;
                }

                return _mapper.Map<ReportScheduleViewModel?>(reportSchedule);
            }
        }

        public async Task<List<ReportSchedule>> GetTodayReportSchedules()
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                return await (unitOfWork.ReportScheduleDAL.GetTodayReportSchedules());
            }
        }

        public async Task<DataPage<ReportScheduleViewModel>> GetReportSchedulePage(ReportScheduleFilterRequest reportScheduleFilterRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                (List<ReportSchedule>, long) pageData = await unitOfWork.ReportScheduleDAL.GetReportSchedulePage(reportScheduleFilterRequest);

                DataPage<ReportScheduleViewModel> reportSchedulePage = new()
                {
                    Data = _mapper.Map<List<ReportScheduleViewModel>>(pageData.Item1),
                    TotalRecords = pageData.Item2
                };

                return reportSchedulePage;
            }
        }

        public async Task<ActionResultResponse<string>> Insert(ReportSchedule newReportSchedule)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                await unitOfWork.ReportScheduleDAL.Insert(newReportSchedule);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "ReportScheduleAddSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Update(UpdateReportScheduleRequest updatedReportSchedule)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                ReportSchedule? reportSchedule = await unitOfWork.ReportScheduleDAL.GetById(updatedReportSchedule.Id);

                if (reportSchedule == null)
                {
                    result.Errors.Add("ReportScheduleDoesNotExist");
                    result.ActionSuccess = false;
                    return result;
                }

                reportSchedule.OfficeId = updatedReportSchedule.OfficeId;
                reportSchedule.ScheduleDate = updatedReportSchedule.ScheduleDate;
                reportSchedule.IsActive = updatedReportSchedule.IsActive;

                await unitOfWork.ReportScheduleDAL.Update(reportSchedule);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "ReportScheduleUpdateSuccess";
                return result;
            }
        }
    }
}

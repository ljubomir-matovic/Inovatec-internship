using AutoMapper;
using Inovatec.OfficeManagementTool.Common.Services.BaseHubService;
using Inovatec.OfficeManagementTool.Common.Services.UserService;
using Inovatec.OfficeManagementTool.Hubs;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class ReportBL : IReportBL
    {
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IBaseHubService<NotificationHub> _hubService;

        public ReportBL(IUnitOfWorkProvider unitOfWorkProvider, IMapper mapper, IUserService userService, IBaseHubService<NotificationHub> hubService)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _mapper = mapper;
            _userService = userService;
            _hubService = hubService;
        }

        public async Task<ActionResultResponse<ReportViewModel?>> GetReportById(int id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<ReportViewModel?> result = new();

                Report? report = await unitOfWork.ReportDAL.GetFullReportById(id);

                if (report == null)
                {
                    throw new Exception("ReportCouldNotBeFound");
                }

                result.ActionSuccess = true;
                result.ActionData = _mapper.Map<ReportViewModel?>(report);
                return result;
            }
        }

        public async Task<ActionResultResponse<DataPage<ReportViewModel>>> GetReports(ReportFilterRequest reportFilterRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<DataPage<ReportViewModel>> result = new();

                (List<Report>, long) pageData = await unitOfWork.ReportDAL.GetReportPage(reportFilterRequest);
                DataPage<ReportViewModel> reportPage = new()
                {
                    
                    Data = _mapper.Map<List<ReportViewModel>>(pageData.Item1),
                    TotalRecords = pageData.Item2
                };

                result.ActionSuccess = true;
                result.ActionData = reportPage;
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Insert(Report newReport)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                long? userId = _userService.GetUserId() ?? throw new Exception("LoggedUserNoIdError");
                newReport.UserId = (long)userId;

                await unitOfWork.ReportDAL.Insert(newReport);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "ReportAddSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Update(UpdateReportRequest updateReportProblemRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Report? report = await unitOfWork.ReportDAL.GetById(updateReportProblemRequest.Id) ?? throw new Exception("ReportProblemDoesNotExist");

                bool changedReportState = report.State != updateReportProblemRequest.State;

                report.Description = updateReportProblemRequest.Description;
                report.State = updateReportProblemRequest.State;

                await unitOfWork.ReportDAL.Update(report);
                await unitOfWork.SaveChangesAsync();

                if (changedReportState)
                {
                    Comment newComment = new()
                    {
                        ReportId = report.Id,
                        UserId = _userService.GetUserId() ?? throw new Exception("LoggedUserNoIdError"),
                        OrderState = report.State,
                        Type = (byte)CommentTypes.StateChange
                    };
                    await unitOfWork.CommentDAL.Insert(newComment);
                    await unitOfWork.SaveChangesAsync();

                    await _hubService.SendMessageToAll("newCommentForReport" + report.Id, _mapper.Map<CommentViewModel?>(await unitOfWork.CommentDAL.GetComment(newComment.Id)));
                }

                result.ActionSuccess = true;
                result.ActionData = "ReportProblemUpdateSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Report? report = await unitOfWork.ReportDAL.GetById(id);

                if (report == null)
                {
                    throw new Exception("ReportDoesNotExist");
                }

                await unitOfWork.ReportDAL.LogicalDelete(id);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "ReportDeleteSuccess";
                return result;
            }
        }
    }
}
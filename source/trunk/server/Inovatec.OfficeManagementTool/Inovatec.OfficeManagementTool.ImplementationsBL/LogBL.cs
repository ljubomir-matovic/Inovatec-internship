using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class LogBL : ILogBL
    {
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IMapper _mapper;

        public LogBL(IUnitOfWorkProvider unitOfWorkProvider, IMapper mapper)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _mapper = mapper;
        }

        public async Task<ActionResultResponse<DataPage<LogViewModel>>> GetLogs(LogsFilterRequest logsFilterRequest)
        {
            using var unitOfWork = _unitOfWorkProvider.Begin();

            ActionResultResponse<DataPage<LogViewModel>> result = new();

            (List<Log>, long) pageData = await unitOfWork.LogDAL.GetLogs(logsFilterRequest);

            DataPage<LogViewModel> logsPage = new()
            {
                Data = _mapper.Map<List<LogViewModel>>(pageData.Item1),
                TotalRecords = pageData.Item2
            };

            result.ActionSuccess = true;
            result.ActionData = logsPage;
            return result;
        }
    }
}

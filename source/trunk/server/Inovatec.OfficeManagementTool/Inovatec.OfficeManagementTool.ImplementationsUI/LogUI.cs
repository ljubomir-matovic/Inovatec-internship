using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class LogUI : ILogUI
    {
        private readonly ILogBL _logBL;

        public LogUI(ILogBL logBL)
        {
            _logBL = logBL;
        }

        public async Task<ActionResultResponse<DataPage<LogViewModel>>> GetLogs(LogsFilterRequest logsFilterRequest)
        {
            return await _logBL.GetLogs(logsFilterRequest);
        }
    }
}

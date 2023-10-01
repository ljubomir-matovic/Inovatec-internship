using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface ILogBL
    {
        Task<ActionResultResponse<DataPage<LogViewModel>>> GetLogs(LogsFilterRequest logsFilterRequest);
    }
}

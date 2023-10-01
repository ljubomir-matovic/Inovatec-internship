using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesUI
{
    public interface ILogUI
    {
        Task<ActionResultResponse<DataPage<LogViewModel>>> GetLogs(LogsFilterRequest logsFilterRequest);
    }
}

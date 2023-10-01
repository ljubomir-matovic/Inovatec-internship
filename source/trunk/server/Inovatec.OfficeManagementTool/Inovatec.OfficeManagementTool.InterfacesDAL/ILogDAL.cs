using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface ILogDAL : IBaseDAL<Log>
    {
        Task<(List<Log>, long)> GetLogs(LogsFilterRequest orderFilterRequest);
    }
}

using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface IReportDAL : IBaseDAL<Report>
    {
        Task<(List<Report>, long)> GetReportPage(ReportFilterRequest reportFilterRequest);
        Task<Report?> GetFullReportById(long id);
    }
}
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface IOfficeDAL : IBaseDAL<Office>
    {
        Task<(List<Office>, long)> GetOfficePage(OfficeFilterRequest officeFilterRequest);

    }
}
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface IEquipmentDAL : IBaseDAL<Equipment>
    {
        Task<(List<EquipmentViewModel>, long)> GetEquipments(EquipmentFilterRequest filterRequest);
    }
}
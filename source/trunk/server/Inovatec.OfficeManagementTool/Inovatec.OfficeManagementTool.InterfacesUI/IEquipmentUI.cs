using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesUI
{
    public interface IEquipmentUI
    {
        Task<DataPage<EquipmentViewModel>> GetEquipments(EquipmentFilterRequest filterRequest, bool currentUser = false);
        Task<ActionResultResponse<string>> Insert(EquipmentCreateRequest entity);
        Task<ActionResultResponse<string>> Update(EquipmentUpdateRequest entity);
        Task<ActionResultResponse<string>> DeleteMore(List<long> ids);
        Task<ActionResultResponse<string>> Delete(long id);
    }
}
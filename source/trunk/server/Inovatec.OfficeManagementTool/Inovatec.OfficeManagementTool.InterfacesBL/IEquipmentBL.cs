using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface IEquipmentBL
    {
        Task<(List<EquipmentViewModel>, long)> GetEquipments(EquipmentFilterRequest filterRequest);
        Task<ActionResultResponse<string>> Insert(EquipmentCreateRequest entity);
        Task<ActionResultResponse<string>> Update(EquipmentUpdateRequest entity);
        Task<ActionResultResponse<string>> DeleteMore(List<long> ids);
        Task<ActionResultResponse<string>> Delete(long id);
    }
}
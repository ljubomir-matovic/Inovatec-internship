using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface ISupplierUI
    {
        Task<SupplierViewModel?> GetSupplierById(long id);

        Task<DataPage<SupplierViewModel>> GetSupplierPage(SupplierFilterRequest filterRequest);

        Task<ActionResultResponse<string>> Insert(SupplierCreateRequest newSupplier);

        Task<ActionResultResponse<string>> Update(UpdateSupplierRequest updatedSupplier);

        Task<ActionResultResponse<string>> Delete(long id);
    }
}

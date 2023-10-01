using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface ISupplierBL
    {
        Task<SupplierViewModel?> GetSupplierById(long id);
        Task<DataPage<SupplierViewModel>> GetSupplierPage(SupplierFilterRequest supplierFilter);
        Task<ActionResultResponse<string>> Insert(Supplier newOffice);
        Task<ActionResultResponse<string>> Update(UpdateSupplierRequest updatedSupplier);
        Task<ActionResultResponse<string>> Delete(long id);
    }
}

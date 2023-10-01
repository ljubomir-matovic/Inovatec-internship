using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface ISupplierDAL : IBaseDAL<Supplier>
    {
        Task<(List<Supplier>, long)> GetSupplierPage(SupplierFilterRequest supplierFilterRequest);
    }
}
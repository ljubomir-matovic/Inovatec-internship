using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class SupplierUI : ISupplierUI
    {
        private readonly ISupplierBL _supplierBL;
        private readonly IMapper _mapper;

        public SupplierUI(ISupplierBL supplierBL, IMapper mapper)
        {
            _supplierBL = supplierBL;
            _mapper = mapper;
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            return await _supplierBL.Delete(id);
        }

        public async Task<SupplierViewModel?> GetSupplierById(long id)
        {
            return await _supplierBL.GetSupplierById(id);
        }

        public async Task<DataPage<SupplierViewModel>> GetSupplierPage(SupplierFilterRequest filterRequest)
        {
            return await _supplierBL.GetSupplierPage(filterRequest);
        }

        public async Task<ActionResultResponse<string>> Insert(SupplierCreateRequest supplierCreateRequest)
        {
            Supplier newSupplier = _mapper.Map<Supplier>(supplierCreateRequest);
            return await _supplierBL.Insert(newSupplier);
        }

        public async Task<ActionResultResponse<string>> Update(UpdateSupplierRequest updatedSupplier)
        {
            return await _supplierBL.Update(updatedSupplier);
        }
    }
}
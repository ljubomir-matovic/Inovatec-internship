using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class SupplierBL : ISupplierBL
    {
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IMapper _mapper;

        public SupplierBL(IUnitOfWorkProvider unitOfWorkProvider, IMapper mapper)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _mapper = mapper;
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {

                ActionResultResponse<string> result = new();

                Supplier? supplier = await unitOfWork.SupplierDAL.GetById(id);

                if (supplier == null)
                {
                    result.Errors.Add("SupplierDoesNotExist");
                    result.ActionSuccess = false;
                    return result;
                }

                await unitOfWork.SupplierDAL.LogicalDelete(id);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "SupplierDeleteSuccess";
                return result;
            }
        }

        public async Task<SupplierViewModel?> GetSupplierById(long id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                Supplier? supplier = await unitOfWork.SupplierDAL.GetById(id);

                if (supplier == null || supplier.IsDeleted == true)
                {
                    return null;
                }
                
                return _mapper.Map<SupplierViewModel?>(supplier);
            }
        }

        public async Task<DataPage<SupplierViewModel>> GetSupplierPage(SupplierFilterRequest filterRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                (List<Supplier>, long) pageData = await unitOfWork.SupplierDAL.GetSupplierPage(filterRequest);

                DataPage<SupplierViewModel> supplierPage = new()
                {
                    Data = _mapper.Map<List<SupplierViewModel>>(pageData.Item1),
                    TotalRecords = pageData.Item2
                };

                return supplierPage;
            }
        }

        public async Task<ActionResultResponse<string>> Insert(Supplier newSupplier)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Supplier? existingSupplier = (await unitOfWork.SupplierDAL
                                            .GetBySpecificProperty(
                                                supplier =>
                                                    supplier.Name.ToLower().Equals(supplier.Name.Trim().ToLower()) &&
                                                    supplier.IsDeleted == false)
                                            ).FirstOrDefault();

                await unitOfWork.SupplierDAL.Insert(newSupplier);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "SupplierAddSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Update(UpdateSupplierRequest updatedSupplier)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Supplier? supplier = await unitOfWork.SupplierDAL.GetById(updatedSupplier.Id);

                if (supplier == null)
                {
                    result.Errors.Add("SupplierDoesNotExist");
                    result.ActionSuccess = false;
                    return result;
                }

                supplier.Name = updatedSupplier.Name;
                supplier.PhoneNumber = updatedSupplier.PhoneNumber;
                supplier.Country = updatedSupplier.Country;
                supplier.City = updatedSupplier.City;
                supplier.Address = updatedSupplier.Address;

                await unitOfWork.SupplierDAL.Update(supplier);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "SupplierUpdateSuccess";
                return result;
            }
        }
    }
}

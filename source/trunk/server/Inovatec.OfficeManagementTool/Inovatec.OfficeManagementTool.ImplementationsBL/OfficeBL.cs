using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class OfficeBL : IOfficeBL
    {
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IMapper _mapper;

        public OfficeBL(IUnitOfWorkProvider unitOfWorkProvider, IMapper mapper)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _mapper = mapper;
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {

                ActionResultResponse<string> result = new();

                Office? office = await unitOfWork.OfficeDAL.GetById(id);

                if (office == null)
                {
                    result.Errors.Add("OfficeDoesNotExist");
                    result.ActionSuccess = false;
                    return result;
                }

                await unitOfWork.OfficeDAL.LogicalDelete(id);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "OfficeDeleteSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<OfficeViewModel?>> GetOfficeById(int id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<OfficeViewModel?> result = new();

                Office? office = await unitOfWork.OfficeDAL.GetById(id);

                if (office == null || office.IsDeleted == true)
                {
                    result.Errors.Add("OfficeCouldNotBeFound");
                    result.ActionSuccess = false;
                    return result;
                }

                result.ActionSuccess = true;
                result.ActionData = _mapper.Map<OfficeViewModel?>(office);
                return result;
            }
        }

        public async Task<ActionResultResponse<DataPage<OfficeViewModel>>> GetOfficePage(OfficeFilterRequest filterRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<DataPage<OfficeViewModel>> result = new();

                (List<Office>, long) pageData = await unitOfWork.OfficeDAL.GetOfficePage(filterRequest);

                DataPage<OfficeViewModel> officePage = new()
                {
                    Data = _mapper.Map<List<OfficeViewModel>>(pageData.Item1),
                    TotalRecords = pageData.Item2
                };

                result.ActionSuccess = true;
                result.ActionData = officePage;
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Insert(Office newOffice)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Office? existingOffice = (await unitOfWork.OfficeDAL
                                            .GetBySpecificProperty(
                                                office =>
                                                    office.Name.ToLower().Equals(newOffice.Name.Trim().ToLower()) &&
                                                    office.IsDeleted == false)
                                            ).FirstOrDefault();

                await unitOfWork.OfficeDAL.Insert(newOffice);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "OfficeAddSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Update(UpdateOfficeRequest updatedOffice)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Office? office = await unitOfWork.OfficeDAL.GetById(updatedOffice.Id);

                if (office == null)
                {
                    result.Errors.Add("OfficeDoesNotExist");
                    result.ActionSuccess = false;
                    return result;
                }

                office.Name = updatedOffice.Name;

                await unitOfWork.OfficeDAL.Update(office);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "OfficeUpdateSuccess";
                return result;
            }
        }
    }
}

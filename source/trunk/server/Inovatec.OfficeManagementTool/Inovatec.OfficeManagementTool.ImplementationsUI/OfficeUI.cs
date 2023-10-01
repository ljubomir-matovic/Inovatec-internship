using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class OfficeUI : IOfficeUI
    {
        private readonly IOfficeBL _officeBL;
        private readonly IMapper _mapper;

        public OfficeUI(IOfficeBL officeBL, IMapper mapper) 
        {
            _officeBL = officeBL;
            _mapper = mapper;
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            return await _officeBL.Delete(id);
        }

        public async Task<ActionResultResponse<OfficeViewModel?>> GetOfficeById(int id)
        {
            return await _officeBL.GetOfficeById(id);
        }

        public async Task<ActionResultResponse<DataPage<OfficeViewModel>>> GetOfficePage(OfficeFilterRequest filterRequest)
        {
            return await _officeBL.GetOfficePage(filterRequest);
        }

        public async Task<ActionResultResponse<string>> Insert(OfficeCreateRequest officeCreateRequest)
        {
            Office newOffice = _mapper.Map<Office>(officeCreateRequest);
            return await _officeBL.Insert(newOffice);
        }

        public async Task<ActionResultResponse<string>> Update(UpdateOfficeRequest updatedOffice)
        {
            return await _officeBL.Update(updatedOffice);
        }
    }
}

using AutoMapper;
using Inovatec.OfficeManagementTool.Common.Services.UserService;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class EquipmentUI : IEquipmentUI
    {
        private readonly IEquipmentBL _equipmentBL;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public EquipmentUI(IEquipmentBL equipmentBL, IMapper mapper, IUserService userService)
        {
            _equipmentBL = equipmentBL;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<DataPage<EquipmentViewModel>> GetEquipments(EquipmentFilterRequest filterRequest, bool currentUser = false)
        {
            if(currentUser)
            {
                filterRequest.UserId = (int) (_userService.GetUserId() ?? -1);
            }
            
            var users = await _equipmentBL.GetEquipments(filterRequest);
            var page = new DataPage<EquipmentViewModel>();
            page.Data = users.Item1;
            page.TotalRecords = users.Item2;
            return page;
        }

        public async Task<ActionResultResponse<string>> Insert(EquipmentCreateRequest entity)
        {
            entity.UserId = null;
            return await _equipmentBL.Insert(entity);
        }

        public async Task<ActionResultResponse<string>> Update(EquipmentUpdateRequest entity)
        {
            return await _equipmentBL.Update(entity);
        }

        public async Task<ActionResultResponse<string>> DeleteMore(List<long> ids)
        {
            return await _equipmentBL.DeleteMore(ids);
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            return await _equipmentBL.Delete(id);
        }
    }
}
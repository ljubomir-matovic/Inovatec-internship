using AutoMapper;
using Inovatec.OfficeManagementTool.Common.Services.UserService;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class EquipmentBL : IEquipmentBL
    {
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public EquipmentBL(
            IUnitOfWorkProvider unitOfWorkProvider,
            IMapper mapper,
            IUserService userService
        )
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<(List<EquipmentViewModel>, long)> GetEquipments(EquipmentFilterRequest filterRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                return await unitOfWork.EquipmentDAL.GetEquipments(filterRequest);
            }
        }


        public async Task<ActionResultResponse<string>> Insert(EquipmentCreateRequest entity)
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                var equipment = _mapper.Map<Equipment>(entity);
                equipment.IsDeleted = false;
                equipment.DateCreated = DateTime.Now;
                equipment.DateModified = DateTime.Now;
                await unitOfWork.EquipmentDAL.Insert(equipment);
                await unitOfWork.SaveChangesAsync();
            }

            result.ActionSuccess = true;
            result.ActionData = "EquipmentCreatedSuccess";
            return result;
        }

        public async Task<ActionResultResponse<string>> Update(EquipmentUpdateRequest entity)
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                foreach(var item in entity.Equipments)
                {
                    Equipment? equipment = await unitOfWork.EquipmentDAL.GetById(item["id"]);

                    if (equipment == null)
                    {
                        result.ActionSuccess = false;
                        result.Errors.Add("EquipmentNotExist");
                        return result;
                    }

                    equipment.ItemId = item["itemId"];
                    equipment.UserId = entity.UserId;

                    await unitOfWork.EquipmentDAL.Update(equipment);
                }
                
                await unitOfWork.SaveChangesAsync();
            }

            result.ActionSuccess = true;

            if(entity.UserId == null)
            {
                result.ActionData = "EquipmentUnassignedSuccess";
            }
            else
            {
                result.ActionData = "EquipmentAssignedSuccess";
            }

            return result;
        }

        public async Task<ActionResultResponse<string>> DeleteMore(List<long> ids)
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                foreach (var id in ids)
                {
                    await unitOfWork.EquipmentDAL.LogicalDelete(id);
                }
                await unitOfWork.SaveChangesAsync();
            }

            result.ActionSuccess = true;
            result.ActionData = "EquipmentDeletedSuccess";
            return result;
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                await unitOfWork.EquipmentDAL.LogicalDelete(id);
                await unitOfWork.SaveChangesAsync();
            }

            result.ActionSuccess = true;
            result.ActionData = "EquipmentDeletedSuccess";
            return result;
        }
    }
}
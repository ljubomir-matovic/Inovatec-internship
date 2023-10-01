using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class ItemBL : IItemBL
    {
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IMapper _mapper;

        public ItemBL(IUnitOfWorkProvider unitOfWorkProvider, IMapper mapper)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _mapper = mapper;
        }

        public async Task<ActionResultResponse<ItemViewModel?>> GetItemById(int id)
        {
            using(var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<ItemViewModel?> result = new();

                Item? item = await unitOfWork.ItemDAL.GetById(id);

                if (item == null || item.IsDeleted == true)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("ItemCouldNotBeFound");
                    return result;
                }

                result.ActionSuccess = true;
                result.ActionData = _mapper.Map<ItemViewModel?>(item);
                return result;
            }
        }

        public async Task<ActionResultResponse<List<ItemViewModel>>> GetItems(ItemFilterRequest itemFilterRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<List<ItemViewModel>> result = new();

                result.ActionData = await unitOfWork.ItemDAL.GetItems(itemFilterRequest);
                result.ActionSuccess = true;

                return result;
            }
        }

        public async Task<ActionResultResponse<DataPage<ItemViewModel>>> GetItemsPage(ItemFilterRequest itemFilterRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<DataPage<ItemViewModel>> result = new();

                (List<ItemViewModel>, long) pageData = await unitOfWork.ItemDAL.GetItemPage(itemFilterRequest);

                DataPage<ItemViewModel> itemPage = new()
                {
                    Data = pageData.Item1,
                    TotalRecords = pageData.Item2
                };

                result.ActionSuccess = true;
                result.ActionData = itemPage;
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Insert(Item newItem)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Item? item = (await unitOfWork.ItemDAL.GetBySpecificProperty(item => item.Name.Trim().ToLower().Equals(newItem.Name.Trim().ToLower()))).FirstOrDefault();
                if (item != null)
                {
                    if (item.IsDeleted == false)
                    {
                        result.ActionSuccess = false;
                        result.Errors.Add("ItemAlreadyExists");
                        return result;
                    }
                    else
                    {
                        item.IsDeleted = false;
                        await Update(_mapper.Map<ItemViewModel>(item));

                        result.ActionSuccess = true;
                        result.ActionData = "ItemAddSuccess";
                        return result;
                    }
                }

                await unitOfWork.ItemDAL.Insert(newItem);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "ItemAddSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Update(ItemViewModel updatedItem)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Item? item = await unitOfWork.ItemDAL.GetById(updatedItem.Id);

                if (item == null)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("ItemDoesNotExist");
                    return result;
                }

                item.Name = updatedItem.Name;
                item.Category = updatedItem.Category;

                await unitOfWork.ItemDAL.Update(item);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "ItemUpdateSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Item? item = await unitOfWork.ItemDAL.GetById(id);

                if (item == null)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("ItemDoesNotExist");
                    return result;
                }

                await unitOfWork.ItemDAL.LogicalDelete(id);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "ItemDeleteSuccess";
                return result;
            }
        }
    }
}
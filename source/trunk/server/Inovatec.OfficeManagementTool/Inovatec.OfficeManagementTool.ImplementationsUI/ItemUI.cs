using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class ItemUI : IItemUI
    {
        private readonly IItemBL _itemBL;
        private readonly IMapper _mapper;

        public ItemUI(IItemBL itemBL, IMapper mapper)
        {
            _itemBL = itemBL;
            _mapper = mapper;
        }

        public async Task<ActionResultResponse<ItemViewModel?>> GetItemById(int id)
        {
            return await _itemBL.GetItemById(id);
        }

        public async Task<ActionResultResponse<List<ItemViewModel>>> GetItems(ItemFilterRequest itemFilterRequest)
        {
            return await _itemBL.GetItems(itemFilterRequest);
        }

        public async Task<ActionResultResponse<DataPage<ItemViewModel>>> GetItemsPage(ItemFilterRequest itemFilterRequest)
        {
            return await _itemBL.GetItemsPage(itemFilterRequest);
        }

        public async Task<ActionResultResponse<string>> Insert(ItemCreateRequest itemCreateRequest)
        {
            Item newItem = _mapper.Map<Item>(itemCreateRequest);
            return await _itemBL.Insert(newItem);
        }

        public async Task<ActionResultResponse<string>> Update(ItemViewModel updatedItem)
        {
            return await _itemBL.Update(updatedItem);
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            return await _itemBL.Delete(id);
        }
    }
}
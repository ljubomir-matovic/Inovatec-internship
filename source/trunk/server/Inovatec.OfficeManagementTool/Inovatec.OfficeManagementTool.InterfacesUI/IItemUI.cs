using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesUI
{
    public interface IItemUI
    {
        Task<ActionResultResponse<ItemViewModel?>> GetItemById(int id);
        Task<ActionResultResponse<DataPage<ItemViewModel>>> GetItemsPage(ItemFilterRequest itemFilter);
        Task<ActionResultResponse<List<ItemViewModel>>> GetItems(ItemFilterRequest itemFilter);
        Task<ActionResultResponse<string>> Insert(ItemCreateRequest newItem);
        Task<ActionResultResponse<string>> Update(ItemViewModel updatedItem);
        Task<ActionResultResponse<string>> Delete(long id);
    }
}
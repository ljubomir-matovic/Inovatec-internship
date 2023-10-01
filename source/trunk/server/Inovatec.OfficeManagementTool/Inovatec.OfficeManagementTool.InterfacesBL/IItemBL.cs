using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface IItemBL
    {
        Task<ActionResultResponse<ItemViewModel?>> GetItemById(int id);
        Task<ActionResultResponse<DataPage<ItemViewModel>>> GetItemsPage(ItemFilterRequest itemFilterRequest);
        Task<ActionResultResponse<List<ItemViewModel>>> GetItems(ItemFilterRequest itemFilterRequest);
        Task<ActionResultResponse<string>> Insert(Item newItem);
        Task<ActionResultResponse<string>> Update(ItemViewModel updatedItem);
        Task<ActionResultResponse<string>> Delete(long id);
    }
}
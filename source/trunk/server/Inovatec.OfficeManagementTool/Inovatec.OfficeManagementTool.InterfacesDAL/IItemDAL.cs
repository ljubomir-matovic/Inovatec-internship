using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface IItemDAL : IBaseDAL<Item>
    {
        Task<List<ItemViewModel>> GetItems(ItemFilterRequest itemFilterRequest);
        Task<(List<ItemViewModel>, long)> GetItemPage(ItemFilterRequest itemFilterRequest);
    }
}

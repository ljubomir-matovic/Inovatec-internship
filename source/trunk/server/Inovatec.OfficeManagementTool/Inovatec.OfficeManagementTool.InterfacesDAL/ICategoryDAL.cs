using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface ICategoryDAL : IBaseDAL<Category>
    {
        Task<List<Category>> GetCategories(CategoryFilterRequest categoryFilter);
        Task<Category?> GetCategory(long id);
        Task<(List<CategoryViewModel>, long)> GetCategoryPage(CategoryFilterRequest categoryFilter);
    }
}
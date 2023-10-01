using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface ICategoryBL
    {
        Task<ActionResultResponse<CategoryViewModel?>> GetCategoryById(int id);
        Task<ActionResultResponse<List<CategoryViewModel>>> GetCategories(CategoryFilterRequest filterRequest);
        Task<ActionResultResponse<string>> Insert(Category newCategory);
        Task<ActionResultResponse<string>> Update(CategoryViewModel updatedCategory);
        Task<ActionResultResponse<string>> Delete(long id);
        Task<ActionResultResponse<DataPage<CategoryViewModel>>> GetCategoryPage(CategoryFilterRequest categoryFilter);
    }
}
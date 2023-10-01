using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Inovatec.OfficeManagementTool.InterfacesUI
{
    public interface ICategoryUI
    {
        Task<ActionResultResponse<CategoryViewModel?>> GetCategoryById(int id);
        Task<ActionResultResponse<List<CategoryViewModel>>> GetCategories(CategoryFilterRequest categoryFilterRequest);
        Task<ActionResultResponse<string>> Insert(CategoryCreateRequest categoryCreateRequest);
        Task<ActionResultResponse<string>> Update(CategoryViewModel updatedCategory);
        Task<ActionResultResponse<string>> Delete(long id);
        Task<ActionResultResponse<DataPage<CategoryViewModel>>> GetCategoryPage(CategoryFilterRequest categoryFilter);
    }
}
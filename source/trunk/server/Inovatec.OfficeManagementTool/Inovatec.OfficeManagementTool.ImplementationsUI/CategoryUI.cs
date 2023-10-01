using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class CategoryUI : ICategoryUI
    {
        private readonly ICategoryBL _categoryBL;
        private readonly IMapper _mapper;

        public CategoryUI(ICategoryBL categoryBL, IMapper mapper)
        {
            _categoryBL = categoryBL;
            _mapper = mapper;
        }

        public async Task<ActionResultResponse<CategoryViewModel?>> GetCategoryById(int id)
        {
            return await _categoryBL.GetCategoryById(id);
        }

        public async Task<ActionResultResponse<List<CategoryViewModel>>> GetCategories(CategoryFilterRequest categoryFilterRequest)
        {
            return await _categoryBL.GetCategories(categoryFilterRequest);
        }

        public async Task<ActionResultResponse<string>> Insert(CategoryCreateRequest categoryCreateRequest)
        {
            Category newCategory = _mapper.Map<Category>(categoryCreateRequest);
            return await _categoryBL.Insert(newCategory);
        }

        public async Task<ActionResultResponse<string>> Update(CategoryViewModel updatedCategory)
        {
            return await _categoryBL.Update(updatedCategory);
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            return await _categoryBL.Delete(id);
        }

        public async Task<ActionResultResponse<DataPage<CategoryViewModel>>> GetCategoryPage(CategoryFilterRequest categoryFilter)
        {
            return await _categoryBL.GetCategoryPage(categoryFilter);
        }
    }
}
using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class CategoryBL : ICategoryBL
    {
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IMapper _mapper;

        public CategoryBL(IUnitOfWorkProvider unitOfWorkProvider, IMapper mapper)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _mapper = mapper;
        }

        public async Task<ActionResultResponse<CategoryViewModel?>> GetCategoryById(int id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<CategoryViewModel?> result = new();

                Category? category = await unitOfWork.CategoryDAL.GetCategory(id);

                if(category == null || category.IsDeleted == true)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("CategoryCouldNotBeFound");
                    return result;
                }

                result.ActionSuccess = true;
                result.ActionData = _mapper.Map<CategoryViewModel?>(category);
                return result;
            }
        }

        public async Task<ActionResultResponse<List<CategoryViewModel>>> GetCategories(CategoryFilterRequest filterRequest)
        {
            using(var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<List<CategoryViewModel>> result = new();

                List<Category> categories = await unitOfWork.CategoryDAL.GetCategories(filterRequest);

                result.ActionData = _mapper.Map<List<CategoryViewModel>>(categories);
                result.ActionSuccess = true;
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Insert(Category newCategory)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Category? category = (await unitOfWork.CategoryDAL.GetBySpecificProperty(category => category.Name.Trim().ToLower().Equals(newCategory.Name.Trim().ToLower()))).FirstOrDefault();
                if(category != null)
                {
                    if (category.IsDeleted == false)
                    {
                        result.ActionSuccess = false;
                        result.Errors.Add("CategoryAlreadyExists");
                        return result;
                    }
                    else
                    {
                        category.IsDeleted = false;
                        await Update(_mapper.Map<CategoryViewModel>(category));

                        result.ActionSuccess = true;
                        result.ActionData = "CategoryAddSuccess";
                        return result;
                    }
                }

                await unitOfWork.CategoryDAL.Insert(newCategory);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "CategoryAddSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Update(CategoryViewModel updatedCategory)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Category? category = await unitOfWork.CategoryDAL.GetById(updatedCategory.Id);
                
                if(category == null)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("CategoryDoesNotExist");
                    return result;
                }

                category.Name = updatedCategory.Name;
                category.Type = updatedCategory.Type;

                await unitOfWork.CategoryDAL.Update(category);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "CategoryUpdateSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<string> result = new();

                Category? category = await unitOfWork.CategoryDAL.GetById(id);

                if(category == null)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("CategoryDoesNotExist");
                    return result;
                }

                await unitOfWork.CategoryDAL.LogicalDelete(id);
                await unitOfWork.SaveChangesAsync();

                result.ActionSuccess = true;
                result.ActionData = "CategoryDeleteSuccess";
                return result;
            }
        }

        public async Task<ActionResultResponse<DataPage<CategoryViewModel>>> GetCategoryPage(CategoryFilterRequest categoryFilter)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                ActionResultResponse<DataPage<CategoryViewModel>> result = new();
                
                (List<CategoryViewModel>, long) pageData = await unitOfWork.CategoryDAL.GetCategoryPage(categoryFilter);

                DataPage<CategoryViewModel> categoryPage = new()
                {
                    Data = pageData.Item1,
                    TotalRecords = pageData.Item2
                };

                result.ActionSuccess = true;
                result.ActionData = categoryPage;
                return result;
            }
        }
    }
}
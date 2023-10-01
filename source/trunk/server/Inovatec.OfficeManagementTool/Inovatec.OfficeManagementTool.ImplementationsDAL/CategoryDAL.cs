using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class CategoryDAL : BaseDAL<OfficeManagementTool_IS2023Context, Category>, ICategoryDAL
    {
        public async Task<List<Category>> GetCategories(CategoryFilterRequest categoryFilter)
        {
            var query = Table
                .Where(category =>
                    (string.IsNullOrEmpty(categoryFilter.Name.Trim()) || category.Name.ToLower().Contains(categoryFilter.Name.ToLower()))
                    && 
                    (categoryFilter.Types.Count == 0 || categoryFilter.Types.Contains(category.Type))
                    &&
                    (category.IsDeleted == false))
                .AsQueryable();

            return await query.ToListAsync();
        }

        public async Task<(List<CategoryViewModel>, long)> GetCategoryPage(CategoryFilterRequest categoryFilter)
        {
            var query = Table
                .Where(category =>
                    (string.IsNullOrEmpty(categoryFilter.Name.Trim()) || category.Name.ToLower().Contains(categoryFilter.Name.ToLower()))
                    &&
                    (categoryFilter.Types.Count == 0 || categoryFilter.Types.Contains(category.Type))
                    &&
                    (category.IsDeleted == false))
                .Select(u => new CategoryViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Type = u.Type
                })
                .AsQueryable();

            long total = query.Count();

            SetOrderBy(categoryFilter.SortField, categoryFilter.SortOrder, ref query);

            query = query.Skip((categoryFilter.PageNumber - 1) * categoryFilter.PageSize).Take(categoryFilter.PageSize);

            return (await query.ToListAsync(), total);
        }

        private void SetOrderBy(string sortField, int sortOrder, ref IQueryable<CategoryViewModel> query)
        {
            Expression<Func<CategoryViewModel, object>> expression;

            switch (sortField)
            {
                case "name":
                    expression = u => u.Name;
                    break;

                default:
                    expression = u => u.Id;
                    break;
            }

            if (sortOrder > 0)
            {
                query = query.OrderBy(expression).AsQueryable();
            }
            else
            {
                query = query.OrderByDescending(expression).AsQueryable();
            }
        }

        public async Task<Category?> GetCategory(long id)
        {
            return await Table.Where(category => category.Id == id && category.IsDeleted == false).FirstOrDefaultAsync();
        }
    }
}
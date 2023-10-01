using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class ItemDAL : BaseDAL<OfficeManagementTool_IS2023Context, Item>, IItemDAL
    {
        public async Task<List<ItemViewModel>> GetItems(ItemFilterRequest itemFilterRequest)
        {
            return await Table
                .Where(item =>
                    (string.IsNullOrEmpty(itemFilterRequest.Name.Trim()) || item.Name.ToLower().Contains(itemFilterRequest.Name.ToLower()))
                    &&
                    (itemFilterRequest.Categories.Count == 0 || itemFilterRequest.Categories.Contains(item.CategoryId))
                    &&
                    (item.Category.Type == itemFilterRequest.CategoryType)
                    &&
                    (item.IsDeleted == false))
                .Select(u => new ItemViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Category = u.Category
                }).ToListAsync();
        }

        public async Task<(List<ItemViewModel>, long)> GetItemPage(ItemFilterRequest itemFilterRequest)
        {
            var query = Table
                .Where(item =>
                    (string.IsNullOrEmpty(itemFilterRequest.Name.Trim()) || item.Name.ToLower().Contains(itemFilterRequest.Name.ToLower()))
                    &&
                    (itemFilterRequest.Categories.Count == 0 || itemFilterRequest.Categories.Contains(item.CategoryId))
                    &&
                    (item.Category.Type == itemFilterRequest.CategoryType)
                    &&
                    (item.IsDeleted == false))
                .Select(u => new ItemViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Category = u.Category
                })
                .AsQueryable();

            long total = query.Count();

            SetOrderBy(itemFilterRequest.SortField, itemFilterRequest.SortOrder, ref query);

            query = query.Skip((itemFilterRequest.PageNumber - 1) * itemFilterRequest.PageSize).Take(itemFilterRequest.PageSize);

            return (await query.ToListAsync(), total);
        }

        private void SetOrderBy(string sortField, int sortOrder, ref IQueryable<ItemViewModel> query)
        {
            Expression<Func<ItemViewModel, object>> expression;

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

        public async Task<Item?> GetItem(long id)
        {
            return await Table.Where(category => category.Id == id && category.IsDeleted == false).FirstOrDefaultAsync();
        }
    }
}
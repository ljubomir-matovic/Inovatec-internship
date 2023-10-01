using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Inovatec.OfficeManagementTool.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class EquipmentDAL : BaseDAL<OfficeManagementTool_IS2023Context, Equipment>, IEquipmentDAL
    {
        private void SetOrderBy(EquipmentFilterRequest filterRequest, long total, ref IQueryable<EquipmentViewModel> query)
        {
            Expression<Func<EquipmentViewModel, object>> expression = filterRequest.SortField switch
            {
                "itemName" => (e => e.ItemName),
                "categoryName" => (e => e.CategoryName),
                _ => (e => e.Id)
            };

            IOrderedQueryable<EquipmentViewModel> orderedQuery;

            if (filterRequest.SortOrder > 0)
            {
                orderedQuery = query.OrderBy(expression);
            }
            else
            {
                orderedQuery = query.OrderByDescending(expression);
            }

            query = orderedQuery.AsQueryable();
        }
        public async Task<(List<EquipmentViewModel>, long)> GetEquipments(EquipmentFilterRequest filterRequest)
        {
            var query = Table
                .Include(equipment => equipment.Item)
                .Include(equipment => equipment.Item.Category)
                .Include(equipment => equipment.User)
                .Where(equipment => !equipment.IsDeleted
                    && equipment.Item.Category.Type == ((byte)CategoryTypes.Equipment)
                    && (filterRequest.UserId != -1 ? equipment.User != null : equipment.User == null)
                    && (filterRequest.UserId <= 0 || equipment.User.Id == filterRequest.UserId)
                    && (filterRequest.ItemId == 0 || equipment.ItemId == filterRequest.ItemId)
                    && (filterRequest.CategoryId == 0 || equipment.Item.Category.Id == filterRequest.CategoryId)
                )
                .Select(equipment => new EquipmentViewModel
                {
                    Id = equipment.Id,
                    ItemId = equipment.ItemId,
                    ItemName = equipment.Item.Name,
                    CategoryId = equipment.Item.Category.Id,
                    CategoryName = equipment.Item.Category.Name,
                    User = equipment.User == null ? null : new UserShortViewModel
                    {
                        Id = equipment.User.Id,
                        FullName = equipment.User.FirstName + " " + equipment.User.LastName,
                        Email = equipment.User.Email
                    }
                    
                })
                .AsQueryable();

            long total = query.Count();

            SetOrderBy(filterRequest,total,ref query);

            query = query
                .Skip((filterRequest.PageNumber - 1) * filterRequest.PageSize)
                .Take(filterRequest.PageSize);

            return (await query.ToListAsync(), total);
        }
    }
}
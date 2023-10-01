using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class OrderItemDAL: BaseDAL<OfficeManagementTool_IS2023Context, OrderItem>, IOrderItemDAL
    {
        private void SetOrderBy(EquipmentFilterRequest filterRequest, long total, ref IQueryable<OrderItemViewModel> query)
        {
            Expression<Func<OrderItemViewModel, object>> expression = filterRequest.SortField switch
            {
                "itemName" => (e => e.Name),
                "categoryName" => (e => e.Category),
                "amount" => (e => e.Amount),
                _ => (e => e.Id)
            };

            IOrderedQueryable<OrderItemViewModel> orderedQuery;

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
        public async Task<(List<OrderItemViewModel>, long)> GetOrderItemsForOrder(long orderId, EquipmentFilterRequest filterRequest)
        {
            var query = Table
                .Include(orderItem => orderItem.Item)
                .Include(orderItem => orderItem.Item.Category)
                .Where(orderItem => !orderItem.IsDeleted
                    && orderItem.OrderId == orderId
                    && (filterRequest.ItemId == 0 || orderItem.ItemId == filterRequest.ItemId)
                    && (filterRequest.CategoryId == 0 || orderItem.Item.Category.Id == filterRequest.CategoryId)
                )
                .Select(orderItem => new OrderItemViewModel
                {
                    Id = orderItem.Id,
                    Name = orderItem.Item.Name,
                    Category = orderItem.Item.Category.Name,
                    Amount = orderItem.Amount
                })
                .AsQueryable();

            long total = query.Count();

            SetOrderBy(filterRequest, total, ref query);

            query = query
                .Skip((filterRequest.PageNumber - 1) * filterRequest.PageSize)
                .Take(filterRequest.PageSize);

            return (await query.ToListAsync(), total);
        }

        public async Task<bool> OrderItemExists(long orderId, long itemId)
        {
            return await Table.Where(orderItem => orderItem.OrderId == orderId 
                && orderItem.ItemId == itemId
                && orderItem.IsDeleted == false).AnyAsync();
        }
    }
}
using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class OrderDAL : BaseDAL<OfficeManagementTool_IS2023Context, Order>, IOrderDAL
    {
        private void SetOrderBy(string? sortField, int sortOrder, ref IQueryable<Order> query)
        {
            Expression<Func<Order, object>> expression = (order => order.DateCreated);

            if (sortOrder > 0)
            {
                query = query
                    .OrderBy(expression)
                    .AsQueryable();
            }
            else
            {
                query = query
                    .OrderByDescending(expression)
                    .AsQueryable();
            }
        }

        public async Task<(List<Order>, long)> GetOrdersPage(OrderFilterRequest orderFilterRequest)
        {
            var query = Table
                .Where(
                    order =>
                        order.IsDeleted == false && order.OfficeId == orderFilterRequest.OfficeId
                )
                .AsQueryable();

            if (orderFilterRequest.States != null && orderFilterRequest.States.Count > 0)
            {
                query = query
                    .Where(order => orderFilterRequest.States.Contains((byte)order.State));
            }

            long total = query.Count();

            SetOrderBy(orderFilterRequest.SortField, orderFilterRequest.SortOrder, ref query);

            if (orderFilterRequest.PageNumber > 0 || orderFilterRequest.PageSize > 0)
            {
                query = query
                    .Skip((orderFilterRequest.PageNumber - 1) * orderFilterRequest.PageSize)
                    .Take(orderFilterRequest.PageSize);
            }

            return (await query.ToListAsync(), total);
        }

        public async Task<List<OrderItem>> GetItemsForOrderById(long id)
        {
            return await Context.OrderItems.Where(orderItem => orderItem.OrderId == id)
                .ToListAsync();  
        }

        public async Task AddOrderItem(OrderItem item)
        {
            await Context.OrderItems.AddAsync(item);
        }

        public async Task<int> CreateOrder(long officeId, object orderId)
        {
            var outputParam = (OutputParameter<int>) orderId;
            return await Context.Procedures.CreateOrderAsync((int?)officeId, outputParam);
        }
    }
}
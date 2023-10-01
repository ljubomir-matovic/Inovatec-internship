using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class OrderRequestDAL : BaseDAL<OfficeManagementTool_IS2023Context, OrderRequest>, IOrderRequestDAL
    {
        public async Task<(List<OrderRequest>, long)> GetActiveOrderPage(OrderFilterRequest orderFilterRequest)
        {
            var query = Table
                .Where(
                    order =>
                        order.Item != null && 
                        order.Description == null &&
                        order.DateTo == null &&
                        order.IsDeleted == false
                )
                .Include(order => order.User)
                .Include(order => order.Item)
                .Include(order => order.Office)
                .AsQueryable();

            if (!string.IsNullOrEmpty(orderFilterRequest.Description))
            {
                query = query
                    .Where(order => order.Description.ToLower().Contains(orderFilterRequest.Description));
            }

            if (!string.IsNullOrEmpty(orderFilterRequest.Name))
            {
                query = query
                    .Where(order => $"{order.User.FirstName.ToLower()} {order.User.LastName.ToLower()}".Contains(orderFilterRequest.Name));
            }

            if (!string.IsNullOrEmpty(orderFilterRequest.Item))
            {
                query = query
                    .Where(order => order.Item.Name.ToLower().Contains(orderFilterRequest.Item));
            }

            if (orderFilterRequest.Users != null && orderFilterRequest.Users.Count > 0)
            {
                query = query
                    .Where(order => orderFilterRequest.Users.Contains(order.UserId));
            }

            if (orderFilterRequest.Items != null && orderFilterRequest.Items.Count > 0)
            {
                query = query
                    .Where(order => orderFilterRequest.Items.Contains((long)order.ItemId));
            }

            if (orderFilterRequest.States != null && orderFilterRequest.States.Count > 0)
            {
                query = query
                    .Where(order => orderFilterRequest.States.Contains((byte)order.State));
            }

            if (orderFilterRequest.Offices != null && orderFilterRequest.Offices.Count > 0)
            {
                query = query
                    .Where(order => orderFilterRequest.Offices.Contains((byte)order.OfficeId));
            }

            long total = query.Count();

            SetOrderBy(orderFilterRequest.SortField, orderFilterRequest.SortOrder, ref query);

            if (orderFilterRequest.PageNumber > 0 && orderFilterRequest.PageSize > 0)
            {
                query = query
                    .Skip((orderFilterRequest.PageNumber - 1) * orderFilterRequest.PageSize)
                    .Take(orderFilterRequest.PageSize);
            }

            return (await query.ToListAsync(), total);
        }

        private void SetOrderBy(string? sortField, int sortOrder, ref IQueryable<OrderRequest> query)
        {
            Expression<Func<OrderRequest, object>> expression = sortField switch
            {
                "description" => (order => order.Description),
                "dateCreated" => (order => order.DateCreated),
                "amount" => (order => order.Amount),
                "name" => (order => order.User.FirstName),
                "item" => (order => order.Item.Name),
                "office" => (order => order.Office.Name),
                _ => (order => order.Id)
            };

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

        public async Task<(List<OrderViewModelGroupByItem>, long)> GetActiveOrdersGroupedByItem(OrderFilterRequest orderFilterRequest)
        {
            var query = Table
                .Where(
                    order => 
                        order.Item != null &&
                        order.Description == null &&
                        order.DateTo == null && 
                        order.IsDeleted == false
                )
                .Include(order => order.User)
                .Include(order => order.Item)
                .AsQueryable();

            if(!string.IsNullOrEmpty(orderFilterRequest.Name))
            {
                query = query
                    .Where(order => $"{order.User.FirstName.ToLower()} {order.User.LastName.ToLower()}".Contains(orderFilterRequest.Name));
            }

            if (!string.IsNullOrEmpty(orderFilterRequest.Item))
            {
                query = query
                    .Where(order => order.Item.Name.ToLower().Contains(orderFilterRequest.Item));
            }

            if (orderFilterRequest.Users != null && orderFilterRequest.Users.Count > 0)
            {
                query = query
                    .Where(order => orderFilterRequest.Users.Contains(order.UserId));
            }

            if (orderFilterRequest.Items != null && orderFilterRequest.Items.Count > 0)
            {
                query = query
                    .Where(order => orderFilterRequest.Items.Contains((long)order.ItemId));
            }

            if (orderFilterRequest.States != null && orderFilterRequest.States.Count > 0)
            {
                query = query
                    .Where(order => orderFilterRequest.States.Contains((byte)order.State));
            }

            if (orderFilterRequest.Offices != null && orderFilterRequest.Offices.Count > 0)
            {
                query = query
                    .Where(order => orderFilterRequest.Offices.Contains((byte)order.OfficeId));
            }

            var groupedQuery = query
                .GroupBy(order => order.ItemId);

            long total = groupedQuery.Count();

            var orderQuery = groupedQuery
                .Select(
                    orders => new OrderViewModelGroupByItem
                    {
                        Item = new ItemViewModel
                        {
                            Id = orders.First().Item.Id,
                            Name = orders.First().Item.Name,
                            Category = orders.First().Item.Category
                        },
                        Amount = orders.Sum(order => order.Amount) ?? 0
                    }
                );

            SetActiveOrdersGroupedByItemOrderBy(orderFilterRequest.SortField, orderFilterRequest.SortOrder, ref orderQuery);

            if (orderFilterRequest.PageNumber > 0 && orderFilterRequest.PageSize > 0)
            {
                orderQuery = orderQuery
                    .Skip((orderFilterRequest.PageNumber - 1) * orderFilterRequest.PageSize)
                    .Take(orderFilterRequest.PageSize);
            }

            return (await orderQuery.ToListAsync(), total);
        }

        private void SetActiveOrdersGroupedByItemOrderBy(string? sortField, int sortOrder, ref IQueryable<OrderViewModelGroupByItem> query)
        {
            Expression<Func<OrderViewModelGroupByItem, object>> expression = sortField switch
            {
                "name" => (order => order.Item.Name),
                "amount" => (order => order.Amount),
                _ => (order => order.Item.Name)
            };

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

        public async Task<OrderRequest?> GetFullOrderById(long id)
        {
            return await Table
                .Include(order => order.User)
                .Include(order => order.Item)
                .Where(order => order.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<SnacksOrderViewModel>> GetSnacksOrderReport(long officeId)
        {
            return await Table
                .Include(order => order.Item)
                .ThenInclude(item => item.Category)
                .Where(order => order.Item != null && order.Item.Category.Type == (byte) CategoryTypes.Snacks && order.DateTo == null && order.OfficeId == officeId)
                .GroupBy(order => order.ItemId)
                .Select(group =>new SnacksOrderViewModel
                    {
                        Name = group.FirstOrDefault().Item.Name,
                        Amount = group.Sum(order => order.Amount ?? 0)
                    })
                .ToListAsync();
        }

        public async Task<List<OrderRequest>> UnacceptedOrders()
        {
            return await Table
                .Include(order => order.Item)
                .ThenInclude(item => item.Category)
                .Where(order => order.Item != null && order.Item.Category.Type == (byte)CategoryTypes.Snacks && order.DateTo == null)
                .ToListAsync();
        }

        public async Task<(List<OrderRequest>, long)> GetEquipmentPage(OrderFilterRequest orderFilterRequest)
        {
            var query = Table
                .Where(
                    order =>
                        order.Item != null &&
                        order.Description != null &&
                        order.IsDeleted == false
                )
                .Include(order => order.User)
                .Include(order => order.Item)
                .AsQueryable();

            if (!string.IsNullOrEmpty(orderFilterRequest.Name))
            {
                query = query
                    .Where(order => $"{order.User.FirstName.ToLower()} {order.User.LastName.ToLower()}".Contains(orderFilterRequest.Name));
            }

            if (!string.IsNullOrEmpty(orderFilterRequest.Description))
            {
                query = query
                    .Where(order => order.Description.Contains(orderFilterRequest.Description));
            }

            if (orderFilterRequest.Users != null && orderFilterRequest.Users.Count > 0)
            {
                query = query
                    .Where(order => orderFilterRequest.Users.Contains(order.UserId));
            }

            if (orderFilterRequest.Items != null && orderFilterRequest.Items.Count > 0)
            {
                query = query
                    .Where(order => orderFilterRequest.Items.Contains((long)order.ItemId));
            }

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

        public async Task<(List<OrderRequest>, long)> GetReportPage(OrderFilterRequest orderFilterRequest)
        {
            var query = Table
                .Where(
                    order =>
                        order.Item == null &&
                        order.Description != null &&
                        order.IsDeleted == false
                )
                .Include(order => order.User)
                .Include(order => order.Item)
                .AsQueryable();

            if (!string.IsNullOrEmpty(orderFilterRequest.Name))
            {
                query = query
                    .Where(order => $"{order.User.FirstName.ToLower()} {order.User.LastName.ToLower()}".Contains(orderFilterRequest.Name));
            }

            if (!string.IsNullOrEmpty(orderFilterRequest.Description))
            {
                query = query
                    .Where(order => order.Description.Contains(orderFilterRequest.Description));
            }

            if (orderFilterRequest.Users != null && orderFilterRequest.Users.Count > 0)
            {
                query = query
                    .Where(order => orderFilterRequest.Users.Contains(order.UserId));
            }

            if (orderFilterRequest.Items != null && orderFilterRequest.Items.Count > 0)
            {
                query = query
                    .Where(order => orderFilterRequest.Items.Contains((long)order.ItemId));
            }

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

        public async Task<List<OrderItem>> GetOrderItems()
        {
            return await Table
                .Include(order => order.Item)
                .ThenInclude(item => item.Category)
                .Where(order => order.Item != null && order.Item.Category.Type == (byte)CategoryTypes.Snacks && order.DateTo == null)
                .GroupBy(order => order.ItemId)
                .Select(orderGroup => new OrderItem
                {
                    ItemId = orderGroup.Key ?? 0,
                    Amount = orderGroup.Sum(order => order.Amount) ?? 0,
                })
                .ToListAsync();
        }

        public async Task<OrderRequest?> GetOrderRequest(long id)
        {
            return await Table
                .Where(order => order.Id == id && order.IsDeleted == false)
                .Include(order => order.Item)
                .Include(order => order.User)
                .Include(order => order.Office)
                .FirstOrDefaultAsync();
        }
    }
}
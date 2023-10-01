using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class SupplierDAL : BaseDAL<OfficeManagementTool_IS2023Context, Supplier>, ISupplierDAL
    {
        public async Task<(List<Supplier>, long)> GetSupplierPage(SupplierFilterRequest supplierFilterRequest)
        {
            var query = Table
                .Where(supplier => supplier.IsDeleted == false)
                .AsQueryable();

            if(!string.IsNullOrEmpty(supplierFilterRequest.Name))
            {
                query = query.Where(supplier => supplier.Name.ToLower().Contains(supplierFilterRequest.Name));
            }

            if (!string.IsNullOrEmpty(supplierFilterRequest.PhoneNumber))
            {
                query = query.Where(supplier => supplier.PhoneNumber.ToLower().Contains(supplierFilterRequest.PhoneNumber));
            }

            if (!string.IsNullOrEmpty(supplierFilterRequest.Country))
            {
                query = query.Where(supplier => supplier.Country.ToLower().Contains(supplierFilterRequest.Country));
            }

            if (!string.IsNullOrEmpty(supplierFilterRequest.City))
            {
                query = query.Where(supplier => supplier.City.ToLower().Contains(supplierFilterRequest.City));
            }

            if (!string.IsNullOrEmpty(supplierFilterRequest.Address))
            {
                query = query.Where(supplier => supplier.Address.ToLower().Contains(supplierFilterRequest.Address));
            }

            long total = query.Count();

            SetOrderBy(supplierFilterRequest.SortField, supplierFilterRequest.SortOrder, ref query);

            if (supplierFilterRequest.PageNumber > 0 && supplierFilterRequest.PageSize > 0)
            {
                query = query
                    .Skip((supplierFilterRequest.PageNumber - 1) * supplierFilterRequest.PageSize)
                    .Take(supplierFilterRequest.PageSize);
            }

            return (await query.ToListAsync(), total);
        }

        private void SetOrderBy(string? sortField, int sortOrder, ref IQueryable<Supplier> query)
        {
            Expression<Func<Supplier, object>> expression = sortField switch
            {
                "name" => (supplier => supplier.Name),
                "phoneNumber" => (supplier => supplier.PhoneNumber),
                "country" => (supplier => supplier.City),
                "city" => (supplier => supplier.Country),
                "address" => (supplier => supplier.Address),
                "date" => (supplier => supplier.DateCreated),
                _ => (supplier => supplier.Id)
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
    }
}

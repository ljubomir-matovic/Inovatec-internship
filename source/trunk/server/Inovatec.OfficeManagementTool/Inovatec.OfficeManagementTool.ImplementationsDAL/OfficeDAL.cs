using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class OfficeDAL : BaseDAL<OfficeManagementTool_IS2023Context, Office>, IOfficeDAL
    {
        public async Task<(List<Office>, long)> GetOfficePage(OfficeFilterRequest officeFilterRequest)
        {
            var query = Table
                .Where(office => office.IsDeleted == false)
                .AsQueryable();

            if(!string.IsNullOrEmpty(officeFilterRequest.Name))
            {
                query = query.Where(office => office.Name.ToLower().Contains(officeFilterRequest.Name));
            }

            long total = query.Count();

            SetOrderBy(officeFilterRequest.SortField, officeFilterRequest.SortOrder, ref query);

            if (officeFilterRequest.PageNumber > 0 && officeFilterRequest.PageSize > 0)
            {
                query = query
                    .Skip((officeFilterRequest.PageNumber - 1) * officeFilterRequest.PageSize)
                    .Take(officeFilterRequest.PageSize);
            }

            return (await query.ToListAsync(), total);
        }

        private void SetOrderBy(string? sortField, int sortOrder, ref IQueryable<Office> query)
        {
            Expression<Func<Office, object>> expression = sortField switch
            {
                "name" => (office => office.Name),
                "date" => (office => office.DateCreated),
                _ => (office => office.Id)
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

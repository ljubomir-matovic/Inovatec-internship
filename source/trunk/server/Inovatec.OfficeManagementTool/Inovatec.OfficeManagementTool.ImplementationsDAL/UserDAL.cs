using Inovatec.OfficeManagementTool.Common.Services.UserService;
using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.Enums;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inovatec.OfficeManagementTool.ImplementationsDAL
{
    public class UserDAL : BaseDAL<OfficeManagementTool_IS2023Context, User>, IUserDAL
    {
        public async Task<long> GetUserIdByEmail(string email)
        {
            return await Table
                .Where(u => u.Email == email)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();
        }

        private void SetOrderBy(string sortField, int sortOrder, ref IQueryable<UserViewModel> query)
        {
            Expression<Func<UserViewModel, object>> expression = sortField switch
            {
                "firstName" => (u => u.FirstName),
                "lastName" => (u => u.LastName),
                "email" => (u => u.Email),
                "office" => (u => u.Office.Name),
                _ => (u => u.Id)
            };

            if (sortOrder > 0)
            {
                query = query.OrderBy(expression).AsQueryable();
            }
            else
            {
                query = query.OrderByDescending(expression).AsQueryable();
            }
        }

        public async Task<(List<UserViewModel>, long)> GetUsers(UserFilterRequest filterRequest, long userId)
        {
            var query = Table
                .Include(u => u.Office)
                .Where(u => 
                !u.IsDeleted
                && u.Id != userId
                && u.FirstName.ToLower().Contains(filterRequest.FirstName) 
                && u.LastName.ToLower().Contains(filterRequest.LastName) 
                && u.Email.ToLower().Contains(filterRequest.Email)
                && u.Office.Name.ToLower().Contains(filterRequest.Office)
                && (filterRequest.Roles.Count == 0 || filterRequest.Roles.Contains(u.Role))
                && (filterRequest.Offices.Count == 0 || filterRequest.Offices.Contains((long) u.OfficeId)))
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    DateCreated = u.DateCreated,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Role = u.Role,
                    Office = u.Office != null ? new OfficeViewModel
                    {
                        Id = u.Office.Id,
                        Name = u.Office.Name,
                        DateCreated = u.Office.DateCreated
                    } : null,
                    OfficeId = (u.OfficeId != null) ? u.OfficeId : null,
                })
                .AsQueryable();

            long total = query.Count();

            SetOrderBy(filterRequest.SortField, filterRequest.SortOrder, ref query);

            filterRequest.PageSize ??= 10;

            query = query
                .Skip((filterRequest.PageNumber - 1) * (int)filterRequest.PageSize)
                .Take((int)filterRequest.PageSize);

            return (await query.ToListAsync(), total);
        }

        public async Task<bool> EmailExists(string email)
        {
            return await Table.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ResetTokenExists(string token)
        {
            return await Table.AnyAsync(u => u.ResetToken == token && u.ResetTokenExpirationTime > DateTime.Now);
        }

        public async Task<User?> GetByToken(string token)
        {
            return await Table.FirstOrDefaultAsync(u => u.ResetToken == token);
        }

        public async Task<bool> UserWithIdExists(long id)
        {
            return await Table.AnyAsync(u => u.Id == id);
        }

        public async Task<List<string>> GetHREmails(long officeId)
        {
            return await Table.Where(u => u.Role == int.Parse(Role.HR) && u.IsDeleted == false && u.OfficeId == officeId).Select(u => u.Email).ToListAsync();
        }

        public async Task<List<UserShortViewModel>> GetHRsForOffice(long officeId)
        {
            return await Table.Where(u => u.Role == int.Parse(Role.HR) && u.OfficeId == officeId)
                .Select(u => new UserShortViewModel
                {
                    Id = u.Id,
                    Email = u.Email
                }).ToListAsync();    
        }

        public async Task<List<string>> GetExistingEmailsFromEmailList(List<string> emails)
        {
            return await Table
                .Where(user => emails.Contains(user.Email))
                .Select(user => user.Email).ToListAsync();
        }

        public async Task<long> GetOfficeId(long userId)
        {
            return (await Table.Where(u => u.Id == userId).Select(u => u.OfficeId).FirstOrDefaultAsync()) ?? 0;
        }
    }
}
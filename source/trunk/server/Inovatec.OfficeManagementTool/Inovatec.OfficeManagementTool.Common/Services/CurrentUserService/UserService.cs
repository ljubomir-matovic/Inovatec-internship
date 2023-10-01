using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Inovatec.OfficeManagementTool.Common.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetClaim(string key)
        {
            return _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value;
        }

        public long? GetUserId()
        {
            Claim? idClaim = _httpContextAccessor.HttpContext.User.FindFirst("Id");
            return idClaim != null && long.TryParse(idClaim.Value, out var userId) ? userId : null;
        }

        public string? GetFullName()
        {
            Claim? nameClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name);
            return nameClaim != null ? nameClaim.Value : null;
        }
    }
}
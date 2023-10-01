using Inovatec.OfficeManagementTool.Common.Services.UserService;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using System.Security.Claims;

namespace Inovatec.OfficeManagementTool.Common.Enricher
{
    public class UserEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserEnricher() : this(new HttpContextAccessor())
        {
        }

        public UserEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            string? fullName = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value; // _userService.GetFullName();
            if (fullName != null) 
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("User", fullName));
            }
        }
    }
}

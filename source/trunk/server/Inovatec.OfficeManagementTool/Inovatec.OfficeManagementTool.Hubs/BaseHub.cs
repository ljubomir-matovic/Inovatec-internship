using Inovatec.OfficeManagementTool.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Inovatec.OfficeManagementTool.Hubs
{
    [Authorize]
    public class BaseHub : Hub
    {
        public const string CurrentUserPrefix = "User";

        public string CurrentUser => CurrentUserPrefix + GetClaim(ClaimName.Id);

        protected string GetClaim(string key)
        {
            return Context.User?.FindFirst(key)?.Value ?? string.Empty;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, CurrentUser);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, CurrentUser);
            await base.OnDisconnectedAsync(exception);
        }

        public string GetConnectionId() => Context.ConnectionId;
    }
}
using Microsoft.AspNetCore.SignalR;

namespace Inovatec.OfficeManagementTool.Common.Services.BaseHubService
{
    public class BaseHubService<THub> : IBaseHubService<THub> where THub : Hub
    {
        private readonly IHubContext<THub> _context;

        public BaseHubService(IHubContext<THub> context)
        {
            _context = context;
        }
        public async Task SendMessageToAll(string eventToListen, object? arg1 = null, object? arg2 = null, object? arg3 = null, object? arg4 = null, object? arg5 = null, object? arg6 = null)
        {
            await _context.Clients.All.SendAsync(eventToListen, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public async Task SendMessageToGroup(string groupName, string eventToListen, object? arg1 = null, object? arg2 = null, object? arg3 = null, object? arg4 = null, object? arg5 = null, object? arg6 = null)
        {
            await _context.Clients.Group(groupName).SendAsync(eventToListen, arg1, arg2, arg3, arg4, arg5, arg6);
        }
    }
}
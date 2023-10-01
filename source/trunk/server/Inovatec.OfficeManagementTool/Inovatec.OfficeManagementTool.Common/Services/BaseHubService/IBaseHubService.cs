using Inovatec.OfficeManagementTool.Models.Entity;
using Microsoft.AspNetCore.SignalR;
using System;

namespace Inovatec.OfficeManagementTool.Common.Services.BaseHubService
{
    public interface IBaseHubService<T> where T : Hub
    {
        Task SendMessageToAll(string eventToListen, object? arg1 = null, object? arg2 = null, object? arg3 = null, object? arg4 = null, object? arg5 = null, object? arg6 = null);
        Task SendMessageToGroup(string groupName, string eventToListen, object? arg1 = null, object? arg2 = null, object? arg3 = null, object? arg4 = null, object? arg5 = null, object? arg6 = null);
    }
}
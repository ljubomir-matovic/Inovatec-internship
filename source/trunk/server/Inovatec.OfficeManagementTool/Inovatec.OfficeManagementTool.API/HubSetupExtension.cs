using Inovatec.OfficeManagementTool.Hubs;

namespace Inovatec.OfficeManagementTool.API
{
    public static class HubSetupExtension
    {
        public static void HubSetup(this WebApplication app)
        {
            app.MapHub<NotificationHub>("/signalR/notification");
        }
    }
}

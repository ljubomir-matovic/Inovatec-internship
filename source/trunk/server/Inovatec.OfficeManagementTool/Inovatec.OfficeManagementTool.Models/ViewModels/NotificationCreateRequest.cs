namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class NotificationCreateRequest
    {
        public long UserId { get; set; }
        public string Email { get; set; }
        public string Data { get; set; }
        public string Description { get; set; } 
        public string Subject { get; set; } 
        public string Body { get; set; } 
        public string Style { get; set; } = string.Empty;
        public long OfficeId { get; set; }
        public bool IncludeSender { get; set; } = true;
    }
}
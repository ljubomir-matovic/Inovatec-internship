namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class NotificationFilterRequest
    {
        public int? UpperBound { get; set; } = -1;
        public bool? IsRead { get; set; } = false;
        public int? PageNumber { get; set; } = 0;
        public int? PageSize { get; set; } = 5;
        public long? UserId { get; set; }
    }
}
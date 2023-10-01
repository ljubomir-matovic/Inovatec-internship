namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class NotificationViewModel
    {
        public long Id { get; set; }
        public string Data { get; set; }
        public string Description { get; set; }
        public bool IsRead { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
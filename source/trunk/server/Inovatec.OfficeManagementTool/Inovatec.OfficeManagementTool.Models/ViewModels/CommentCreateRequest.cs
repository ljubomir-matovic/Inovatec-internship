namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class CommentCreateRequest
    {
        public string Text { get; set; } = string.Empty;
        public long UserId { get; set; }
        public long? OrderId { get; set; }
        public long? ReportId { get; set; }
    }
}
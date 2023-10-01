namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class UpdateReportScheduleRequest
    {
        public long Id { get; set; }
        public long OfficeId { get; set; }
        public DateTimeOffset ScheduleDate { get; set; }
        public bool IsActive { get; set; }
    }
}

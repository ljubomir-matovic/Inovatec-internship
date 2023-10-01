using Inovatec.OfficeManagementTool.Models.Entity;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class ReportScheduleViewModel
    {
        public long Id { get; set; }
        public OfficeViewModel Office { get; set; }
        public DateTimeOffset ScheduleDate { get; set; }
        public bool IsScheduled { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class ReportScheduleCreateRequest
    {
        public long OfficeId { get; set; }
        public DateTimeOffset ScheduleDate { get; set; }
    }
}

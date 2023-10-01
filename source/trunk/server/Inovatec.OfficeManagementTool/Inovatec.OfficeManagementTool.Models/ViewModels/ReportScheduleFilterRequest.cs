using Inovatec.OfficeManagementTool.Models.Enums;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class ReportScheduleFilterRequest
    {
        public List<long>? Offices { get; set; } = new List<long>();

        public int State { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string? SortField { get; set; } = string.Empty;

        public int SortOrder { get; set; } = 1;
    }
}
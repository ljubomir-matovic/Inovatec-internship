namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class EquipmentFilterRequest
    {
        public int? ItemId { get; set; } = 0;
        public int? CategoryId { get; set; } = 0;
        public int? UserId { get; set; } = 0;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortField { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 1;
    }
}
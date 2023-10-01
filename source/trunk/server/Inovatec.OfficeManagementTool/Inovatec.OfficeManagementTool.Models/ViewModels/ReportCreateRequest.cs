namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class ReportCreateRequest
    {
        public long? EquipmentId { get; set; }
        public string Description { get; set; } = string.Empty;
        public byte Category { get; set; }
        public long OfficeId { get; set; }
    }
}
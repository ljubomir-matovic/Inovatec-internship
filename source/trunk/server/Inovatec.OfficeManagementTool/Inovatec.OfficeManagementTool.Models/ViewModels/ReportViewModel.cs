using Inovatec.OfficeManagementTool.Models.Entity;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class ReportViewModel
    {
        public long Id { get; set; }
        public UserViewModel User { get; set; }
        public ReportedEquipmentViewModel Equipment { get; set; }
        public string Description { get; set; }
        public byte? State { get; set; }
        public byte? Category { get; set; }
        public DateTime? DateCreated { get; set; }
        public OfficeViewModel Office { get; set; }
    }
}
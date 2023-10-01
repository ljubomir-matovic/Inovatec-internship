using Inovatec.OfficeManagementTool.Models.Entity;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class ReportedEquipmentViewModel
    {
        public long Id { get; set; }
        public ItemViewModel Item { get; set; }
        public UserViewModel User { get; set; }
    }
}
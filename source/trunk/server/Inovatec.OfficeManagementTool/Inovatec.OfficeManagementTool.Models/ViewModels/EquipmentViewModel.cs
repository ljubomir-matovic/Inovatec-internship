using Inovatec.OfficeManagementTool.Models.Entity;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class EquipmentViewModel
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public UserShortViewModel? User { get; set; }
    }
}
using Inovatec.OfficeManagementTool.Models.Entity;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class EquipmentOrderCreateRequest
    {
        public long ItemId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
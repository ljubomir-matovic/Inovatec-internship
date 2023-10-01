namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class UpdateEquipmentOrderRequest
    {
        public long Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public byte State { get; set; }
    }
}
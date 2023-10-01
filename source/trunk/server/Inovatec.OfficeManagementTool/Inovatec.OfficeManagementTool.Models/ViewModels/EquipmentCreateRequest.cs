namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class EquipmentCreateRequest
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public long? UserId {  get; set; }
    }
}
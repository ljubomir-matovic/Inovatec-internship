namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class EquipmentUpdateRequest
    {
        public List<Dictionary<string,long>> Equipments { get; set; }
        public long? UserId { get; set; }
    }
}
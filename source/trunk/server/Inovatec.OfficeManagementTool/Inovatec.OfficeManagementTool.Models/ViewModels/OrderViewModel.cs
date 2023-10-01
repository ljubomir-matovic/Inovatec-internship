using Inovatec.OfficeManagementTool.Models.Entity;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class OrderViewModel
    {
        public long Id { get; set; }
        public UserViewModel User { get; set; }
        public ItemViewModel Item { get; set; }
        public int? Amount { get; set; }
        public string Description { get; set; }
        public byte? State { get; set; }
        public DateTime? DateCreated { get; set; }
        public OfficeViewModel Office { get; set; }
    }
}
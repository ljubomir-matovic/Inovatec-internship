namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class OrderItemCreateRequest
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long ItemId { get; set; }
        public int Amount { get; set; }
    }
}
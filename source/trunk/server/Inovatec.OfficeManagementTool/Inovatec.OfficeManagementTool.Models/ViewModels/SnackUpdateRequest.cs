namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class SnackUpdateRequest
    {
        public long OrderId { get; set; }
        public int Amount { get; set; }
        public long OfficeId { get; set; }
    }
}
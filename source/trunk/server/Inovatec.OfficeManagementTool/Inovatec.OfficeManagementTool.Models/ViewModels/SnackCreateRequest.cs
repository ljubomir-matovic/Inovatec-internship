using Inovatec.OfficeManagementTool.Models.Entity;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class SnackCreateRequest
    {
        public long UserId { get; set; }
        public long ItemId { get; set; }
        public int Amount { get; set; }
        public long OfficeId { get; set; }
    }
}
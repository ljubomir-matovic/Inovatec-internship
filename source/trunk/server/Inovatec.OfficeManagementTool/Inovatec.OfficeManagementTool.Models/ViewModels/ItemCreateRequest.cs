namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class ItemCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public long CategoryId { get; set; }
    }
}
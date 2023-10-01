using Inovatec.OfficeManagementTool.Models.Entity;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class ItemViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
    }
}
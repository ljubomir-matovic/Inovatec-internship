using System.ComponentModel.DataAnnotations;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class ChangePersonalData
    {
        [Required]
        public string FirstName { get; set; } = null;
        [Required]
        public string LastName { get; set; } = null;
        public string? Email { get; set; } = null;
        public long? RoleId { get; set; } = null;
    }
}
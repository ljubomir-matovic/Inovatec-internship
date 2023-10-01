using System.ComponentModel.DataAnnotations;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class UserCreateRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public int Role { get; set; }

        public long OfficeId { get; set; }
    }
}
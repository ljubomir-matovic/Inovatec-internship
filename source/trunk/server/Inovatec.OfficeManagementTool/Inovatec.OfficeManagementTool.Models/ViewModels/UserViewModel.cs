using Inovatec.OfficeManagementTool.Models.Entity;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class UserViewModel 
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Role { get; set; }
        public OfficeViewModel Office { get; set; }
        public long? OfficeId { get; set; }
    }
}
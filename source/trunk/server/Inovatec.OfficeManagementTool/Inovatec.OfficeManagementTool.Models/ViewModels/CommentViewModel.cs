using Inovatec.OfficeManagementTool.Models.Entity;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class CommentViewModel
    {
        public long Id { get; set; }
        public UserViewModel User { get; set; } = new UserViewModel();
        public string Text{ get; set; } = string.Empty;
        public byte OrderState { get; set; }
        public byte Type { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
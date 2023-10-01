using Inovatec.OfficeManagementTool.Models.Entity;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class LoginResponse
    {
        public UserViewModel UserData { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
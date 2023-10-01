using System.ComponentModel.DataAnnotations;

namespace Inovatec.OfficeManagementTool.Models.ViewModels
{
    public class ForgotPasswordRequest
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
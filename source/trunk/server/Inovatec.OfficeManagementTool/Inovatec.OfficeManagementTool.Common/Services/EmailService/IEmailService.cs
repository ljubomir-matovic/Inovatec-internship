namespace Inovatec.OfficeManagementTool.Common.Services.EmailService
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string destination, string subject, string style,string body);
    }
}
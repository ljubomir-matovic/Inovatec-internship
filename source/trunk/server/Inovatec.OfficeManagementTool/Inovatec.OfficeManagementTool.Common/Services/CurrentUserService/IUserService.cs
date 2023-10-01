namespace Inovatec.OfficeManagementTool.Common.Services.UserService
{
    public interface IUserService
    {
        long? GetUserId();
        string? GetClaim(string key);
        string? GetFullName();
    }
}
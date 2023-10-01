using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesDAL
{
    public interface IUserDAL : IBaseDAL<User>
    {
        Task<(List<UserViewModel>, long)> GetUsers(UserFilterRequest filterRequest, long userId);

        Task<long> GetUserIdByEmail(string email);

        Task<bool> EmailExists(string email);

        Task<bool> ResetTokenExists(string token);

        Task<User?> GetByToken(string token);

        Task<bool> UserWithIdExists(long id);

        Task<List<UserShortViewModel>> GetHRsForOffice(long officeId);

        Task<List<string>> GetHREmails(long officeId);

        Task<List<string>> GetExistingEmailsFromEmailList(List<string> emails);

        Task<long> GetOfficeId(long userId);
    }
}
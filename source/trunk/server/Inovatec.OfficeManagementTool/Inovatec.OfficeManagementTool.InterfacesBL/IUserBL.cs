using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface IUserBL
    {
        Task<User?> LoginUser(LoginRequest loginRequest);
        Task<string> GetNewUserToken(User user);
        Task<UserViewModel?> GetPersonalData();
        Task<User?> GetUserById(int id);
        Task<(List<UserViewModel>,long)> GetUsers(UserFilterRequest filterRequest);
        Task<ActionResultResponse<string>> Insert(UserCreateRequest entity);
        Task<ActionResultResponse<string>> Update(UserViewModel entity);
        Task<ActionResultResponse<string>> Delete(long id);
        Task<ActionResultResponse<string>> CreateResetToken(string email);
        Task<ActionResultResponse<string>> ResetPassword(string token, string password);
        Task<ActionResultResponse<string>> ChangePassword(string oldPassword, string newPassword);
        Task<ActionResultResponse<string>> ChangeFirstOrLastName(ChangePersonalData requestBody);
        Task<ActionResultResponse<string>> AddUsersFromCSV(CSVUploadRequest uploadRequest);
        Task<byte[]> GetCSVTemplate();
        Task<List<UserShortViewModel>> GetHRsForOffice(long officeId);
    }
}
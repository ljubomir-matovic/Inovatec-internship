using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesUI
{
    public interface IUserUI
    {
        Task<ActionResultResponse<LoginResponse?>> LoginUser(LoginRequest loginRequest);

        Task<UserViewModel?> GetUserByIdForAdmin(int id);

        Task<UserViewModel?> GetPersonalData();

        Task<DataPage<UserViewModel>> GetUsers(UserFilterRequest queryParams);

        Task<byte[]> GetCSVTemplate();

        Task<ActionResultResponse<string>> AddUsersFromCSV(CSVUploadRequest uploadRequest);

        Task<ActionResultResponse<string>> Insert(UserCreateRequest entity);

        Task<ActionResultResponse<string>> Update(UserViewModel entity);

        Task<ActionResultResponse<string>> Delete(long id);

        Task<ActionResultResponse<string>> CreateResetToken(string email);

        Task<ActionResultResponse<string>> ResetPassword(string token, string password);

        Task<ActionResultResponse<string>> ChangePassword(ChangePasswordRequest requestBody);

        Task<ActionResultResponse<string>> ChangeFirstOrLastName(ChangePersonalData requestBody);
    }
}
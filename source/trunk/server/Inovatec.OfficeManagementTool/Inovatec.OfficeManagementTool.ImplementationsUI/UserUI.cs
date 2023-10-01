using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using Inovatec.OfficeManagementTool.Models.Entity;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class UserUI : IUserUI
    {
        private readonly IUserBL _userBL;
        private readonly IMapper _mapper;

        public UserUI(IUserBL userBL, IMapper mapper)
        {
            _userBL = userBL;
            _mapper = mapper;
        }
        
        public async Task<UserViewModel?> GetUserByIdForAdmin(int id)
        {
            var user = await _userBL.GetUserById(id);
            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<DataPage<UserViewModel>> GetUsers(UserFilterRequest queryParams)
        {
            var users = await _userBL.GetUsers(queryParams);
            DataPage<UserViewModel> adminPage = new DataPage<UserViewModel>();
            adminPage.Data = users.Item1;
            adminPage.TotalRecords = users.Item2 ;
            return adminPage;
        }

        public async Task<ActionResultResponse<string>> Insert(UserCreateRequest entity)
        {
            return await _userBL.Insert(entity);
        }

        public async Task<ActionResultResponse<string>> Update(UserViewModel entity)
        {
            return await _userBL.Update(entity);
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            return await _userBL.Delete(id);
        }

        public async Task<ActionResultResponse<string>> CreateResetToken(string email)
        {
            return await _userBL.CreateResetToken(email);
        }

        public async Task<ActionResultResponse<string>> ResetPassword(string token, string password)
        {
            return await _userBL.ResetPassword(token, password);
        }
        public async Task<ActionResultResponse<LoginResponse?>> LoginUser(LoginRequest loginRequest)
        {
            ActionResultResponse<LoginResponse?> result = new();

            User? user = await _userBL.LoginUser(loginRequest);

            if (user != null)
            {
                LoginResponse response = new LoginResponse();
                response.UserData = _mapper.Map<UserViewModel>(user);
                response.Token = await _userBL.GetNewUserToken(user);
                result.ActionData = response;
                result.ActionSuccess = true;
            }
            else
            {
                result.ActionSuccess = false;
                result.Errors.Add("Invalid credentials!");
            }

            return result;
        }

        public async Task<ActionResultResponse<string>> ChangePassword(ChangePasswordRequest requestBody)
        {
            return await _userBL.ChangePassword(requestBody.OldPassword, requestBody.NewPassword);
        }

        public async Task<ActionResultResponse<string>> ChangeFirstOrLastName(ChangePersonalData requestBody)
        {
            return await _userBL.ChangeFirstOrLastName(requestBody);
        }

        public async Task<UserViewModel?> GetPersonalData()
        {
            return await _userBL.GetPersonalData();
        }

        public async Task<ActionResultResponse<string>> AddUsersFromCSV(CSVUploadRequest uploadRequest)
        {
            return await _userBL.AddUsersFromCSV(uploadRequest);
        }

        public async Task<byte[]> GetCSVTemplate()
        {
            return await _userBL.GetCSVTemplate();
        }
    }
}
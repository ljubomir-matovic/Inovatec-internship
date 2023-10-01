using AutoMapper;
using CsvHelper;
using Inovatec.OfficeManagementTool.Common;
using Inovatec.OfficeManagementTool.Common.Services.EmailService;
using Inovatec.OfficeManagementTool.Common.Services.UserService;
using Inovatec.OfficeManagementTool.ImplementationsBL.Helpers;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;
using System.Globalization;
using System.Text;

namespace Inovatec.OfficeManagementTool.ImplementationsBL
{
    public class UserBL : IUserBL
    {
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public UserBL(
            IUnitOfWorkProvider unitOfWorkProvider,
            IMapper mapper,
            IEmailService emailService,
            IUserService userService
        )
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _mapper = mapper;
            _emailService = emailService;
            _userService = userService;
        }

        public async Task<UserViewModel?> GetPersonalData()
        {
            int id = (int)(_userService.GetUserId() ?? 0);

            return _mapper.Map<UserViewModel?>(await GetUserById(id));
        }

        public async Task<User?> GetUserById(int id)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                return await unitOfWork.UserDAL.GetById(id);
            }
        }

        public async Task<(List<UserViewModel>, long)> GetUsers(UserFilterRequest filterRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                return await unitOfWork.UserDAL.GetUsers(filterRequest, _userService.GetUserId() ?? 0);
            }
        }

        private async Task<(string, DateTime)> GenerateResetTokenAsync()
        {
            string token = Guid.NewGuid().ToString();
            DateTime expirationTime = DateTime.Now.AddMinutes(ConfigProvider.ResetTokenExpirationTimeInMinutes);
            return (token, expirationTime);
        }

        private async Task<bool> SendResetTokenOnEmail(string email, string token)
        {
            string url = ConfigProvider.ClientUrl + "/reset-password?token=" + token;
            string body = @"<p style='padding: 20px;font-size:16px;'>
            You can confirm your password on <a href='{0}'>this site</a>.<br><br>
            Best regards,<br>
            Inovatec team.
            </p>
            ";
            return await _emailService.SendEmail(email, "Reset password", string.Empty, string.Format(body, url));
        }

        public async Task<ActionResultResponse<string>> Insert(UserCreateRequest entity)
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                if (await unitOfWork.UserDAL.EmailExists(entity.Email))
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("EmailExists");
                    return result;
                }

                User user = _mapper.Map<User>(entity);
                user.DateCreated = DateTime.Now;
                user.IsDeleted = false;
                user.Password = "";
                user.IsActive = false;
                var tokenAndExpirationTime = await GenerateResetTokenAsync();
                user.ResetToken = tokenAndExpirationTime.Item1;
                user.ResetTokenExpirationTime = tokenAndExpirationTime.Item2;
                await unitOfWork.UserDAL.Insert(user);
                await unitOfWork.SaveChangesAsync();

                if (!await SendResetTokenOnEmail(user.Email, user.ResetToken))
                {
                    result.ActionSuccess = false;
                    result.ActionData = "EmailNotSent";
                    return result;
                }
            }

            result.ActionSuccess = true;
            result.ActionData = "UserCreatedSuccess";
            return result;
        }

        public async Task<ActionResultResponse<string>> Update(UserViewModel entity)
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                User? user = await unitOfWork.UserDAL.GetById(entity.Id);

                if (user == null)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("UserNotExist");
                    return result;
                }

                long id = await unitOfWork.UserDAL.GetUserIdByEmail(entity.Email);
                if (id != 0 && user.Id != id)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("EmailExists");
                    return result;
                }

                user.FirstName = entity.FirstName;
                user.LastName = entity.LastName;
                user.Email = entity.Email;
                user.Role = entity.Role;
                user.OfficeId = entity.OfficeId;

                await unitOfWork.UserDAL.Update(user);
                await unitOfWork.SaveChangesAsync();
            }

            result.ActionSuccess = true;
            result.ActionData = "UserUpdatedSuccess";
            return result;
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                if (!await unitOfWork.UserDAL.UserWithIdExists(id))
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("UserNotExist");
                    return result;
                }
                await unitOfWork.UserDAL.LogicalDelete(id);
                await unitOfWork.SaveChangesAsync();
            }

            result.ActionSuccess = true;
            result.ActionData = "UserDeletedSuccess";
            return result;
        }

        public async Task<ActionResultResponse<string>> CreateResetToken(string email)
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                var users = await unitOfWork.UserDAL.GetBySpecificProperty(u => u.Email == email);
                
                if(users.Count == 0)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("UserNotExist");
                    return result;
                }

                var user = users[0];

                if (user.ResetTokenExpirationTime != null && user.ResetTokenExpirationTime > DateTime.Now)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("TokenAlreadyCreated");
                    return result;
                }

                var tokenAndExpirationTime = await GenerateResetTokenAsync();
                user.ResetToken = tokenAndExpirationTime.Item1;
                user.ResetTokenExpirationTime = tokenAndExpirationTime.Item2;

                await unitOfWork.UserDAL.Update(user);
                await unitOfWork.SaveChangesAsync();

                if (!await SendResetTokenOnEmail(user.Email, user.ResetToken))
                {
                    result.ActionSuccess = false;
                    result.ActionData = "EmailNotSent";
                    return result;
                }
            }

            result.ActionSuccess = true;
            result.ActionData = "aa";
            return result;
        }

        public async Task<ActionResultResponse<string>> ResetPassword(string token, string password)
        {
            ActionResultResponse<string> result = new ActionResultResponse<string>();

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                User? user = await unitOfWork.UserDAL.GetByToken(token);
                if(user == null) 
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("This token was already used!");
                    return result;
                }
                
                if(user.ResetTokenExpirationTime < DateTime.Now)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("This token has expired!");
                    return result;
                }

                user.ResetToken = null;
                user.ResetTokenExpirationTime = null;
                user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                if(user.IsActive == false)
                {
                    user.IsActive = true;
                }
                await unitOfWork.UserDAL.Update(user);
                await unitOfWork.SaveChangesAsync();
            }
            result.ActionSuccess = true;
            result.ActionData = "Password has been changed successfully!";
            return result;
        }

        public async Task<User?> LoginUser(LoginRequest loginRequest)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                User? user = (await unitOfWork.UserDAL.GetBySpecificProperty(user => user.Email.Equals(loginRequest.Email))).FirstOrDefault();
                if (user == null || user.IsDeleted == true || user.IsActive == false)
                {
                    return null;
                }

                bool authenticated = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password);
                if (!authenticated)
                {
                    return null;
                }

                return user;
            }
        }

        public async Task<string> GetNewUserToken(User user)
        {
            return await JwtService.GetToken(user);
        }

        public async Task<ActionResultResponse<string>> ChangePassword(string oldPassword, string newPassword)
        {
            int id = (int)(_userService.GetUserId() ?? 0);
            var result = new ActionResultResponse<string>();
            var user = await GetUserById(id);

            if (user == null)
            {
                result.ActionSuccess = false;
                result.Errors.Add("User doesn't exist.");
                return result;
            }

            if (BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
            {
                using(var unitOfWork = _unitOfWorkProvider.Begin())
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    await unitOfWork.UserDAL.Update(user);
                    await unitOfWork.SaveChangesAsync();
                }
                
                result.ActionSuccess = true;
                result.ActionData = "PasswordChangedSuccess";
            }
            else
            {
                result.ActionSuccess = false;
                result.Errors.Add("WrongOldPassword");
            }

            return result;
        }

        public async Task<ActionResultResponse<string>> ChangeFirstOrLastName(ChangePersonalData requestBody)
        {
            int id = (int)(_userService.GetUserId() ?? 0);

            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                var result = new ActionResultResponse<string>();

                var user = await GetUserById(id);

                if (user == null)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("User doesn't exist");
                    return result;
                }

                user.FirstName = requestBody.FirstName; 
                user.LastName = requestBody.LastName;
                await unitOfWork.UserDAL.Update(user);
                await unitOfWork.SaveChangesAsync();

                return result;
            }
        }

        public async Task<ActionResultResponse<string?>> AddUsersFromCSV(CSVUploadRequest uploadRequest)
        {
            ActionResultResponse<string?> result = new ActionResultResponse<string?>
            {
                ActionSuccess = true,
                ActionData = null,
                Errors = new List<string>()
            };

            if (uploadRequest.Files.Count < 1)
            {
                result.ActionSuccess = false;
                result.Errors.Add("NoCSVFilesError");
                return result;
            }

            if (!uploadRequest.Files[0].ContentType.Equals("text/csv"))
            {
                result.ActionSuccess = false;
                result.Errors.Add("InvalidFileTypeError");
                return result;
            }

            using (var reader = new StreamReader(uploadRequest.Files[0].OpenReadStream()))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                List<UserCreateRequest> users = csvReader.GetRecords<UserCreateRequest>().ToList();

                if(users.Count == 0)
                {
                    result.ActionSuccess = false;
                    result.Errors.Add("NoCSVDataError");
                    return result;
                }

                await InsertUsersList(users);
            }

            result.ActionData = "AddUsersFromCSVSuccess";
            return result;
        }

        private async Task InsertUsersList(List<UserCreateRequest> users)
        {
            using (var unitOfWork = _unitOfWorkProvider.Begin())
            {
                List<string> existingUserEmails = await unitOfWork.UserDAL.GetExistingEmailsFromEmailList(users.Select(user => user.Email).ToList());

                List<UserCreateRequest> filteredUserCreateRequests = users.Where(user => !existingUserEmails.Contains(user.Email)).ToList();

                if (filteredUserCreateRequests.Count > 0)
                {
                    foreach (var userCreateRequest in filteredUserCreateRequests)
                    {
                        User user = _mapper.Map<User>(userCreateRequest);

                        var tokenAndExpirationTime = await GenerateResetTokenAsync();
                        user.Password = "";
                        user.ResetToken = tokenAndExpirationTime.Item1;
                        user.ResetTokenExpirationTime = tokenAndExpirationTime.Item2;

                        await unitOfWork.UserDAL.Insert(user);
                        _ = SendResetTokenOnEmail(user.Email, user.ResetToken);
                    }
                    await unitOfWork.SaveChangesAsync();
                }
            }
        }

        public async Task<byte[]> GetCSVTemplate()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteHeader<UserCreateRequest>();
                }

                return memoryStream.ToArray();
            }
        }

        public async Task<List<UserShortViewModel>> GetHRsForOffice(long officeId)
        {
            using(var unitOfWork = _unitOfWorkProvider.Begin())
            {
                return await unitOfWork.UserDAL.GetHRsForOffice(officeId);
            }
        }
    }
}
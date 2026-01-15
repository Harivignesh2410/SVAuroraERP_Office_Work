namespace SVAuroraERP.Infrastructure.Repositories.Authentication
{
    public class UserServiceRepository : IUserServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        
        private readonly ILogger<UserServiceRepository> _logger;
        private readonly IGlobalConfigServiceRepository _globalConfigServiceRepository;
        private readonly ITransLogRespository _transLogRespository;
        public UserServiceRepository(SVAuroraERPDbContext dbContext,
                            ILogger<UserServiceRepository> logger,
                            IGlobalConfigServiceRepository globalConfigServiceRepository,
                            ITransLogRespository transLogRespository)
        {
            _dbcontext = dbContext;
            _logger = logger;
            _globalConfigServiceRepository = globalConfigServiceRepository;
            _transLogRespository = transLogRespository;
        }
        public async Task<List<UserLoginData>> GetUsersList()
        {
            return await _dbcontext.vUserLoginData.OrderBy(o => o.UserName).ToListAsync();
        }
        public async Task<UserLoginData> GetUserByID(int UserID)
        {
            return await _dbcontext.vUserLoginData.FirstOrDefaultAsync(w => w.UserID == UserID);
        }
        public async Task<Tuple<bool, string,int>> SaveUser(User request)
        {
            bool IsSuccess = false;
            string SuccessMessage = string.Empty;
            int NewUserID = 0;

            try
            {
                if (request == null || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.PasswordHash) || request.RoleID <= 0)
                {
                    return Tuple.Create(false, "Invalid data. Please check the user details.",NewUserID);
                }

                // Check if another user already has the same username (but a different UserID)
                var existingUser = await _dbcontext.User.FirstOrDefaultAsync(w => w.UserName == request.UserName);
                if (existingUser != null)
                {
                    return Tuple.Create(false, "Username already exists!", NewUserID);
                }

                var config = await _globalConfigServiceRepository.GetGlobalConfig();
                if (config == null) return Tuple.Create(false, "Configuration Not Found.", NewUserID);

                string EncryptionKey = config.EncryptionKey;
                request.PasswordHash = Core.Security.SecurityService.Encrypt(request.PasswordHash, EncryptionKey);

                _dbcontext.User.Add(request);
                await _dbcontext.SaveChangesAsync();
                NewUserID = request.UserID;

                IsSuccess = true;
                SuccessMessage = "Saved Successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError($"StreetLightSurvey.Infrastructure.Repositories.Authentication.UserServiceRepository.SaveRole()-> Error: {ex.Message}, StackTrace: {ex.StackTrace}");

                IsSuccess = false;
                SuccessMessage = Constants.ExceptionMessage;
            }

            return Tuple.Create(IsSuccess, SuccessMessage, NewUserID);
        }
        public async Task<Tuple<bool, string,int>> UpdateUser(User request)
        {
            bool IsSuccess = false;
            string SuccessMessage = string.Empty;
            int NewUserID = request.UserID;

            try
            {
                if (request == null || string.IsNullOrEmpty(request.UserName)  || request.RoleID <= 0)
                {
                    return Tuple.Create(false, "Invalid data. Please check the user details.", NewUserID);
                }

                // Check if another user already has the same username (but a different UserID)
                var existingUser = await _dbcontext.User.FirstOrDefaultAsync(w => w.UserName == request.UserName && w.UserID != request.UserID);
                if (existingUser != null)
                {
                    return Tuple.Create(false, "Username already exists!", NewUserID);
                }

                // Find the user to update
                var userToUpdate = await _dbcontext.User.FirstOrDefaultAsync(w => w.UserID == request.UserID);
                if (userToUpdate == null) return Tuple.Create(false, "User not found.", NewUserID);

                // Update user properties
                userToUpdate.UserName = request.UserName;
                userToUpdate.FirstName = request.FirstName;
                userToUpdate.LastName = request.LastName;
                userToUpdate.Email = request.Email;
                userToUpdate.RoleID = request.RoleID;
                userToUpdate.LandingPageID = request.LandingPageID;
                userToUpdate.IsActive = request.IsActive;
                await _dbcontext.SaveChangesAsync();

                IsSuccess = true;
                SuccessMessage = "Updated Successfully";
                NewUserID = userToUpdate.UserID;
            }
            catch (Exception ex)
            {
                _logger.LogError($"StreetLightSurvey.Infrastructure.Repositories.Authentication.UserServiceRepository.UpdateUser()-> Error: {ex.Message}, StackTrace: {ex.StackTrace}");

                IsSuccess = false;
                SuccessMessage = Constants.ExceptionMessage;
            }

            return Tuple.Create(IsSuccess, SuccessMessage, NewUserID);
        }
        public async Task<Tuple<bool, string>> DeleteUser(int UserID, long LoginAuditID)
        {
            bool IsSuccess = false;
            string SuccessMessage = string.Empty;

            try
            {
                // Find the user to update
                var userToDelete = await _dbcontext.User.FirstOrDefaultAsync(w => w.UserID == UserID);
                if (userToDelete == null) return Tuple.Create(false, "User not found.");

                // Update user properties
                userToDelete.IsDeleted = true;
                await _dbcontext.SaveChangesAsync();

                IsSuccess = true;
                SuccessMessage = "Deleted Successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError($"StreetLightSurvey.Infrastructure.Repositories.Authentication.UserServiceRepository.DeleteUser()-> Error: {ex.Message}, StackTrace: {ex.StackTrace}");

                IsSuccess = false;
                SuccessMessage = Constants.ExceptionMessage;
            }

            return Tuple.Create(IsSuccess, SuccessMessage);
        }
        public async Task<DataResponse> ChangePassword(ChangePassword request)
        {
            DataResponse DataResponse = new DataResponse();

            // Validate new password and confirm password
            if (request.NewPassword != request.ConfirmPassword)
            {
                DataResponse.Error = true;
                DataResponse.Success = false;
                DataResponse.ID = request.UserID;
                DataResponse.Message = "New Password and Confirm Password do not match!";
                return DataResponse;
            }

            var checkifexists = await _dbcontext.User.FirstOrDefaultAsync(w => w.UserID == request.UserID && w.IsActive == true);

            if (checkifexists == null)
            {
                DataResponse.Error = true;
                DataResponse.Success = false;
                DataResponse.ID = request.UserID;
                DataResponse.Message = Constants.InvalidLoginSession;
                return DataResponse;
            }

            string EncryptionKey = (await _globalConfigServiceRepository.GetGlobalConfig()).EncryptionKey;
            string CurrentPassword = Core.Security.SecurityService.Decrypt(checkifexists.PasswordHash, EncryptionKey);

            // Check if the current password is correct
            if (CurrentPassword != request.CurrentPassword)
            {
                DataResponse.Error = true;
                DataResponse.Success = false;
                DataResponse.ID = request.UserID;
                DataResponse.Message = Constants.InvalidPassword;
                return DataResponse;
            }

            // Encrypt and update the new password
            checkifexists.PasswordHash = Core.Security.SecurityService.Encrypt(request.NewPassword, EncryptionKey);
            await _dbcontext.SaveChangesAsync();

            DataResponse.Message = Constants.UpdatedSucessfully;
            DataResponse.ID = checkifexists.UserID;
            DataResponse.Success = true;

            return DataResponse;
        }
        public async Task<DataResponse> UpdateUserName(User request)
        {
            DataResponse DataResponse = new DataResponse();

            var checkifexists = await _dbcontext.User.FirstOrDefaultAsync(w => w.UserID == request.UserID && w.IsActive == true);

            if (checkifexists == null)
            {
                DataResponse.Error = true;
                DataResponse.Success = false;
                DataResponse.ID = request.UserID;
                DataResponse.Message = Constants.InvalidLoginSession;
                return DataResponse;
            }

            
            checkifexists.FirstName = request.FirstName;
            checkifexists.LastName = request.LastName;
            checkifexists.Email = request.Email;
            checkifexists.LandingPageID = request.LandingPageID;
            checkifexists.UserProfilePicURL = request.UserProfilePicURL;
            await _dbcontext.SaveChangesAsync();

            DataResponse.Message = Constants.UpdatedSucessfully;
            DataResponse.ID = checkifexists.UserID;
            DataResponse.Success = true;

            return DataResponse;
        }
        public async Task<UserLoginData> GetUserProfile(int UserID)
        {
            var userdetails= await _dbcontext.vUserLoginData.FirstOrDefaultAsync(w => w.UserID == UserID);
            return userdetails;
        }
        public DataResponse UploadProfilePicture(int UserID, string UserProfilePicURL)
        {
            DataResponse DataResponse = new DataResponse();

            var checkIfDataExists = _dbcontext.User.FirstOrDefault(w => w.UserID == UserID);

            if (checkIfDataExists == null)
            {
                DataResponse.Error = true;
                DataResponse.Success = false;
                DataResponse.ID = UserID;
                DataResponse.Message = Constants.NoRecordFound;

                return DataResponse;
            }
            
            checkIfDataExists.UserProfilePicURL = UserProfilePicURL;
            _dbcontext.SaveChanges();

            DataResponse.ID = UserID;
            DataResponse.Message = Constants.SuccessMessage;

            return DataResponse;
        }

        public async Task<DataResponse> ChangePasswordAdminAsync(int userID, string newPassword)
        {
            DataResponse DataResponse = new DataResponse();

                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = userID;
                    DataResponse.Message = "New password is required.";
                    return DataResponse;
                }

                var checkifexists = await _dbcontext.User.FirstOrDefaultAsync(w => w.IsDeleted==false && w.UserID == userID && w.IsActive == true);

                if (checkifexists == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = userID;
                    DataResponse.Message = Constants.InvalidLoginSession;
                    return DataResponse;
                }

                string EncryptionKey = (await _globalConfigServiceRepository.GetGlobalConfig()).EncryptionKey;
                // Encrypt and update the new password
                checkifexists.PasswordHash = Core.Security.SecurityService.Encrypt(newPassword, EncryptionKey);
                await _dbcontext.SaveChangesAsync();

                DataResponse.Message = Constants.UpdatedSucessfully;
                DataResponse.ID = checkifexists.UserID;
                DataResponse.Success = true;

                return DataResponse;
            
        }
        public DataResponse GetUserDataTable(UserDataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<UserLoginData> query = _dbcontext.vUserLoginData;

                if (request.ApplicationID > 0)
                {
                    query = query.Where(d => d.ApplicationID == request.ApplicationID);
                }
                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.UserName.Contains(request.SearchValue) ||
                                             d.RoleName.Contains(request.SearchValue) ||
                                             d.FirstName.Contains(request.SearchValue) ||
                                             d.LastName.Contains(request.SearchValue) ||
                                             d.PageName.Contains(request.SearchValue) 
                                             );
                }
               
                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.vUserLoginData.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {

                                                       w.UserID,
                                                       w.UserName,
                                                       w.FirstName,
                                                       w.LastName,
                                                       w.PageName,
                                                       w.IsActive,
                                                       w.RoleName
                                                   }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                // _auditLogger.SaveActionLog("VListDispatchOrderTrans", ActionType.ListData, null, null, null, "ListDispatchedOrdersServiceRepository.GetListDispatchOrdersDetails()");
            }
            catch (Exception ex)
            {
                // response = _errorLoggerService.LogException(ex, request, "ListDispatchedOrdersServiceRepository.GetListDispatchOrdersDetails()");
            }
            return response;
        }
    }
}

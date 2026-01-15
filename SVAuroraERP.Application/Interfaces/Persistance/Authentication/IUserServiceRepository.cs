namespace SVAuroraERP.Application.Interfaces.Persistance.Authentication
{
    public interface IUserServiceRepository
    {
        Task<List<UserLoginData>> GetUsersList();
        Task<UserLoginData> GetUserByID(int UserID);
        Task<Tuple<bool, string,int>> SaveUser(User user);
         Task<Tuple<bool, string, int>> UpdateUser(User request);
        Task<Tuple<bool, string>> DeleteUser(int UserID, long LoginAuditID);
        Task<DataResponse> ChangePassword(ChangePassword request);
        DataResponse UploadProfilePicture(int UserID, string ProfilePicturePath);
        Task<DataResponse> UpdateUserName(User request);
        Task<UserLoginData> GetUserProfile(int UserID);
        Task<DataResponse> ChangePasswordAdminAsync(int userID, string newPassword);
        DataResponse GetUserDataTable(UserDataTableRequest request);
    }
}
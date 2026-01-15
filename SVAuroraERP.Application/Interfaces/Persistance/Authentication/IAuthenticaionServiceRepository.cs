using Core.Logging.Models;

namespace SVAuroraERP.Application.Interfaces.Persistance.Authentication
{
    public interface IAuthenticaionServiceRepository
    {
        List<LkupMenuControl> GetMenuControl();
        List<LkupPageControl> GetPageControl(int MenuID);
        Task<UserLoginData?> SignIn(LoginAudit request);
        void SignOut(long LoginAuditID);
    }
}
using SVAuroraERP.Application.Interfaces;

namespace SVAuroraERP.WebUI.Custom
{
    //Added on 2025.07.09
    public class AppUserContext : IAppUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AppUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public UserLoginData? GetUser()
        {
            return _httpContextAccessor.HttpContext?.Items["LoggedUser"] as UserLoginData;
        }
    }
}
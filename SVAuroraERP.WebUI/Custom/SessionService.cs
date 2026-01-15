namespace SVAuroraERP.WebUI.Custom
{
    public class SessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SessionService> _logger;
        public SessionService(IHttpContextAccessor httpContextAccessor, ILogger<SessionService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public UserLoginData? GetLoggedUser()
        {
            var userData = _httpContextAccessor.HttpContext?.Session.GetObjectFromJson<UserLoginData>("UserLoggedData");

            return userData;
        }

        public VHSRPUser? GetVHSRPUser()
        {
            var vhsrpUser = _httpContextAccessor.HttpContext?.Session.GetObjectFromJson<VHSRPUser>("VHSRPUserData");
            return vhsrpUser;
        }
    }
}
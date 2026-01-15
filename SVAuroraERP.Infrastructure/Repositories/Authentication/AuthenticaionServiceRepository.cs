namespace SVAuroraERP.Infrastructure.Repositories.Authentication
{
    public class AuthenticaionServiceRepository : IAuthenticaionServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly SVAuroraERPLogDbContext _logDBContext;
        private readonly IGlobalConfigServiceRepository _repository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public AuthenticaionServiceRepository(SVAuroraERPDbContext dbcontext, IGlobalConfigServiceRepository repository,
            IErrorLoggerService errorLoggerService, IAuditLogger auditLogger, SVAuroraERPLogDbContext logDBContext)
        {
            _dbcontext = dbcontext;
            _repository = repository;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
            _logDBContext = logDBContext;
        }

        public List<LkupMenuControl> GetMenuControl()
        {
            var menuList = _dbcontext.LkupMenuControl.OrderBy(o => o.OrdinalNo).Select(s => new LkupMenuControl
            {
                MenuControlID = s.MenuControlID,
                ModuleID = s.ModuleID,
                MenuName = s.MenuName,
                MenuDisplayName = s.MenuDisplayName,
                MenuIcon = s.MenuIcon,
                OrdinalNo = s.OrdinalNo,
                MenuGroupID = s.MenuGroupID
            }).ToList();

            return menuList;
        }

        public List<LkupPageControl> GetPageControl(int MenuID)
        {
            var pagelist = _dbcontext.LkupPageControl.Where(w => w.MenuControlID == MenuID && w.IsVisible == true)
                        .OrderBy(o => o.OrdinalNo).Select(s => new LkupPageControl
                        {
                            PageControlID = s.PageControlID,
                            MenuControlID = s.MenuControlID,
                            PageIcon = s.PageIcon,
                            PageName = s.PageName,
                            PageURL = s.PageURL,
                            OrdinalNo = s.OrdinalNo,
                            IsVisible = s.IsVisible,
                        }).ToList();

            return pagelist;
        }

        public async Task<UserLoginData?> SignIn(LoginAudit request)
        {
            try
            {
                var userdata = await _dbcontext.User.FirstOrDefaultAsync(w => w.UserName == request.UserName);

                UserLoginData? userLoginData = null;

                if (userdata != null)
                {
                    string? EncryptionKey = (await _repository.GetGlobalConfig()).EncryptionKey;

                    if (request.UserPassword == Core.Security.SecurityService.Decrypt(userdata.PasswordHash, EncryptionKey))
                    {
                        userLoginData = await _dbcontext.vUserLoginData.Where(w => w.UserID == userdata.UserID).SingleOrDefaultAsync();

                        request.UserID = userdata.UserID;
                        request.LoginDate = DateTime.UtcNow;

                        userLoginData.LastLoginDate = _auditLogger.GetLastUserLoginDate(request.UserID);//Added on 2025.07.15
                        userLoginData.LoginAuditID = _auditLogger.SaveLoginAuditInfo(request);

                        if (userLoginData.ApplicationID == (byte)Common.Application.HSRPPortal)
                        {
                            userLoginData.HSRPUser = await _dbcontext.VHSRPUser.Where(w => w.UserID == userdata.UserID).FirstOrDefaultAsync();
                            userLoginData.HSRPUser.LoginAuditID = userLoginData.LoginAuditID;
                        }
                    }
                }
                return userLoginData;
            }
            catch (Exception ex)
            {
                var dataresponse = _errorLoggerService.LogException(ex, request, "AuthenticateServiceRepository.SignIn()");
                return null;
            }
        }

        public void SignOut(long LoginAuditID) => _auditLogger.SignOut(LoginAuditID);
    }
}
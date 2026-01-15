namespace SVAuroraERP.Infrastructure.Repositories.Orders.Import.Logger
{
    public class AuditLogger : IAuditLogger
    {
        private readonly SVAuroraERPLogDbContext _logdbContext;
        private readonly ApplicationSettings _appSettings;
        private readonly IAppUserContext _appuserContext;
        public AuditLogger(SVAuroraERPLogDbContext logdbContext, ApplicationSettings appSettings, IAppUserContext appuserContext)
        {
            _logdbContext = logdbContext;
            _appSettings = appSettings;
            _appuserContext = appuserContext;
        }

        public void SaveAPIRequestLog(APIRequestLog request)
        {
            var logger = new ProjectLogger(_logdbContext);

            logger.SaveAPIRequestLog(request);
        }

        public long SaveLoginAuditInfo(LoginAudit request)
        {
            var logger = new ProjectLogger(_logdbContext);

            request.ProjectCode = _appSettings.ProjectCode;
            request = logger.SaveLoginAuditInfo(request);

            return request.LoginAuditID;
        }

        public void SaveErrorLog(ErrorLog logError)
        {
            var logger = new ProjectLogger(_logdbContext);
            logError.ProjectCode = _appSettings.ProjectCode;

            logger.SaveErrorLog(logError);
        }

        public void InsertPageAccessAuditLog(PageAccessAudit request)
        {
            var logger = new ProjectLogger(_logdbContext);
            logger.SavePageAccessAudit(request);
        }

        public void SignOut(long LoginAuditID)
        {
            var usrLoginAuditData = _logdbContext.LoginAuditInfo.FirstOrDefault(x => x.LoginAuditID == LoginAuditID);

            if (usrLoginAuditData != null)
            {
                usrLoginAuditData.LogoutDate = DateTime.UtcNow;
                _logdbContext.SaveChanges();
            }
        }

        //Added on 2025.07.15
        public DateTime? GetLastUserLoginDate(int UserID)
        {
            int ProjectID = _logdbContext.Project.FirstOrDefault(w => w.ProjectCode.ToString() == _appSettings.ProjectCode).ProjectID;

            var lastLoginDateold = _logdbContext.LoginAuditInfo
                                .Where(w => w.UserID == UserID && w.ProjectID == ProjectID)
                                .OrderByDescending(o => o.LoginAuditID)
                                .Select(x => x.LoginDate) // project only the field you need
                                .FirstOrDefault();

            var lastLoginDate = _logdbContext
                        .LoginAuditInfo
                        .FromSqlRaw(@"
                            SELECT TOP 1 dbo.ConvertUtcToIst(LoginDate) AS LoginDate
                            FROM tLoginAudit
                            WHERE FK_UserID = {0} AND FK_ProjectID = {1}
                            ORDER BY PK_LoginAuditID DESC", UserID, ProjectID)
                        .Select(x => x.LoginDate)
                        .FirstOrDefault();


            return lastLoginDate;
        }
        //Added on 2025.07.23 by Harivignesh
        public void SaveActionLog(string? _entityName, ActionType _actionType, string? _primaryKeyID, object? _requestData = null, object? _previousData = null, string _methodName = null)
        {
            var logger = new ProjectLogger(_logdbContext);

            var actionlogRequest = new ActionLog()
            {
                LoginAuditID = _appuserContext.GetUser().LoginAuditID,
                EntityName = _entityName,
                PrimaryKeyID = _primaryKeyID,
                ActionType = _actionType.ToString(),
                RequestObject = _requestData,
                PreviousObject = _previousData,
                ActionLogDate = DateTime.UtcNow,
                MethodName = _methodName
            };
            logger.SaveActionLog(actionlogRequest);
        }
    }
}
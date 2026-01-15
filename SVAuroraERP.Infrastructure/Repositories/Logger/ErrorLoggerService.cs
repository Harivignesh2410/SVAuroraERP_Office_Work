namespace SVAuroraERP.Infrastructure.Repositories.Orders.Import.Logger
{
    public class ErrorLoggerService : IErrorLoggerService
    {
        private readonly IAuditLogger _auditLogger;
        private readonly IAppUserContext _appuserContext;

        public ErrorLoggerService(IAuditLogger auditLogger, IAppUserContext appUserContext)
        {
            _auditLogger = auditLogger;
            _appuserContext = appUserContext;
        }
        public DataResponse LogException(Exception ex, object requestObject, string methodName)
        {
            DataResponse response = new DataResponse();
            response.Error = true;
            response.Success = false;

            string errorID = Guid.NewGuid().ToString();
            response.Message = $"An error occurred. Please contact support with Error ID: {errorID}";

            var loginAuditId = _appuserContext.GetUser()?.LoginAuditID;

            var errorLog = new ErrorLog
            {
                LoginAuditID = loginAuditId,
                MethodName = methodName,
                ErrorID = errorID,
                RequestObject = requestObject,
                RequestData = "",
                ErrorMessage = ex.Message + " | " + ex.InnerException,
                StackTrace = ex.StackTrace,
                ErrorLoggedDate = DateTime.UtcNow
            };

            _auditLogger.SaveErrorLog(errorLog);

            return response;
        }
    }
}
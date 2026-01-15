using Core.Logging.Models;

namespace SVAuroraERP.Application.Interfaces.Persistance.Logger
{
    public interface IAuditLogger
    {
        public long SaveLoginAuditInfo(LoginAudit request);
        public void SaveErrorLog(ErrorLog logError);
        public void InsertPageAccessAuditLog(PageAccessAudit request);
        public void SignOut(long LoginAuditID);

        //Added on 2025.07.15
        public DateTime? GetLastUserLoginDate(int UserID);

        //Added on 2025.07.23 by Harivignesh
        public void SaveActionLog(string? _entityName, ActionType _actionType, string? _primaryKeyID, object? _requestData = null, object? _previousData = null, string? _methodName = null);
    }
}
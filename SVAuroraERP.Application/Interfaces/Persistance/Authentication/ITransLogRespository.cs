using Core.Logging.Models;

namespace SVAuroraERP.Application.Interfaces.Persistance.Authentication
{
    public interface ITransLogRespository
    {
        void SaveLogTransaction(long LoginAuditID, string TableName, string LogID, ActionType actionType);
    }
}
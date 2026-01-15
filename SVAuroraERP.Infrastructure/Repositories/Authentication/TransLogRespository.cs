namespace SVAuroraERP.Infrastructure.Repositories.Authentication
{
    public class TransLogRespository : ITransLogRespository
    {
        private readonly SVAuroraERPDbContext _dbcontext;

        public TransLogRespository(SVAuroraERPDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public void SaveLogTransaction(long LoginAuditID, string TableName, string LogID, ActionType actionType)
        {
            Domain.Logging.TransactionLog log = new Domain.Logging.TransactionLog();
            log.LoginAuditID = LoginAuditID;
            log.TableName = TableName;
            log.LogID = LogID;
            log.ActionTypeID = (byte)actionType;

            _dbcontext.TransactionLog.Add(log);
            _dbcontext.SaveChanges();
        }
    }
}
namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Production
{
    public class ProcessTypeServiceRepository : IProcessTypeServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<IProcessTypeServiceRepository> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public ProcessTypeServiceRepository(SVAuroraERPDbContext dbcontext, ILogger<IProcessTypeServiceRepository> logger, IAuditLogger auditLogger, 
                                            IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public ProcessTypeServiceRepository(SVAuroraERPDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public List<VProcessType> GetProcessTypeList()
        {
            try
            {
                _auditLogger.SaveActionLog("VProcessType", ActionType.ListData, null, null,null, "ProcessTypeServiceRepository.GetProcessTypeList()");
                return _dbcontext.VProcessType.OrderBy(o => o.ProcessTypeID).ToList();
            }
            catch (Exception ex)
            {
                _errorLoggerService.LogException(ex, null,"ProcessTypeServiceRepository.GetProcessTypeList()");
                return null;
            }

        }
        public VProcessType GetByID(int ID)
        {
            try
            {
                var resultdata = _dbcontext.VProcessType.FirstOrDefault(w => w.ProcessTypeID == ID);

                _auditLogger.SaveActionLog("VProcessType", ActionType.ListData, null, ID,null, "ProcessTypeServiceRepository.GetProcessTypeList()");
                return resultdata;
            }
            catch (Exception ex)
            {
                _errorLoggerService.LogException(ex, ID, "ProcessTypeServiceRepository.GetProcessTypeList()");
                return null;
            }
        }
        public Tuple<bool, bool> Update(ProcessType request)
        {
            bool IsSuccess = false;
            bool doesItemExist = false;
            try
            {
                var dataexists = _dbcontext.ProcessType.FirstOrDefault(r => r.ProcessTypeID == request.ProcessTypeID);
                if (dataexists != null && !doesItemExist)
                {
                    dataexists.ProcessTypeID = request.ProcessTypeID;
                    dataexists.ProcessDescription = request.ProcessDescription;
                    dataexists.OutputComponentTypeID = request.OutputComponentTypeID;

                    _dbcontext.SaveChanges();
                    IsSuccess = true;
                    _auditLogger.SaveActionLog("ProcessType", ActionType.Update, request.ProcessTypeID.ToString(), request, dataexists, "ProcessTypeServiceRepository.Update()");
                }
                else
                    doesItemExist = false;
               
            }
            catch (Exception ex)
            {
                _errorLoggerService.LogException(ex, request, "ProcessTypeServiceRepository.Update()");
                IsSuccess = false;
            }
            return Tuple.Create(IsSuccess, doesItemExist);
        }
    }
}
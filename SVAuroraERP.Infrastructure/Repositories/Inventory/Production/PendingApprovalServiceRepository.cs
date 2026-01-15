namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Production
{
    public class PendingApprovalServiceRepository : IPendingApprovalFilterServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<PendingApprovalServiceRepository> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;

        public PendingApprovalServiceRepository(SVAuroraERPDbContext dbcontext,
                                                 ILogger<PendingApprovalServiceRepository> logger,
                                                 IAuditLogger auditLogger,
                                                 IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;

        }

        public List<VStockRequest> GetPendingApprovalByFilter(PendingApprovalFilter searchFilter)
        {
            try
            {
                var query = _dbcontext.VStockRequest.AsQueryable();

                if (searchFilter.ProcessTypeID > 0) query = query.Where(o => o.ProcessTypeID == searchFilter.ProcessTypeID);
                if (!string.IsNullOrEmpty(searchFilter.sStartDate) && !string.IsNullOrEmpty(searchFilter.sEndDate)) query = query.Where(o => o.RequestDate >= searchFilter.StartDate
                                                                                                                                        && o.RequestDate <= searchFilter.EndDate);

                if (!string.IsNullOrEmpty(searchFilter.SearchInWord))
                {
                    string keyword = searchFilter.SearchInWord.ToLower(); // Convert to lowercase for case-insensitive search

                    query = query.Where(o =>
                        o.StockRequestID.ToString().Contains(keyword) ||
                        o.ProcessTypeName != null && o.ProcessTypeName.ToLower().Contains(keyword) ||
                          o.RequestedByName != null && o.RequestedByName.ToLower().Contains(keyword) ||
                          o.RequestNo != null && o.RequestNo.ToLower().Contains(keyword)
                    );
                }
                _auditLogger.SaveActionLog("VStockRequest", ActionType.ListData, null, searchFilter,null, "PendingApprovalServiceRepository.GetPendingApprovalByFilter()");
                return query.Where(w => w.StatusID == 1).OrderBy(o => o.RequestNo).ToList();
            }

            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, searchFilter, "PendingApprovalServiceRepository.GetPendingApprovalByFilter()");
                return null;
            }
        }
        public Tuple<bool, bool> ApproveorRejectStockRequest(ApprovalRequest request)
        {
            bool IsSuccess = false;
            bool doesTaxExist = false;
            try
            {
                IsSuccess = ApproveRejectStockRequest(request) > 0 ? true : false;
                _auditLogger.SaveActionLog("VStockRequest", ActionType.ListData, null, request, null, "PendingApprovalServiceRepository.ApproveorRejectStockRequest()");
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, request, "PendingApprovalServiceRepository.ApproveorRejectStockRequest()");   
            }
            _auditLogger.SaveActionLog("VStockRequest", ActionType.ListData, null, request);
            return Tuple.Create(IsSuccess, doesTaxExist);
        }
        private int ApproveRejectStockRequest(ApprovalRequest request)
        {
            int id = 0;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.UPDATESTOCKREQUESTSTATUS;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var pkStockRequestIDParam = command.CreateParameter();
                        pkStockRequestIDParam.ParameterName = "@PK_StockRequestID";
                        pkStockRequestIDParam.Value = request.StockRequestID;

                        var fkstatusIDParam = command.CreateParameter();
                        fkstatusIDParam.ParameterName = "@Fk_StatusID";
                        fkstatusIDParam.Value = request.StatusID;

                        var lastUpdatedByParam = command.CreateParameter();
                        lastUpdatedByParam.ParameterName = "@LastUpdatedBy";
                        lastUpdatedByParam.Value = request.LastUpdatedBy;

                        var narrationParam = command.CreateParameter();
                        narrationParam.ParameterName = "@Narration";
                        narrationParam.Value = request.Narration;

                        command.Parameters.Add(pkStockRequestIDParam);
                        command.Parameters.Add(fkstatusIDParam);
                        command.Parameters.Add(lastUpdatedByParam);
                        command.Parameters.Add(narrationParam);

                        id = command.ExecuteNonQuery();
                    }
                }
                _auditLogger.SaveActionLog("StockRequest", ActionType.Insert, null, request,null, "PendingApprovalServiceRepository.ApproveRejectStockRequest()");
                
            }
            catch(Exception ex)
            {
                _errorLoggerService.LogException(ex, request, "PendingApprovalServiceRepository.ApproveRejectStockRequest()");
            }
            return id;
        }

        public List<VStockRequest> GetProductionInwardByFilter(PendingApprovalFilter searchFilter)
        {
            try
            {
                var query = _dbcontext.VStockRequest.AsQueryable();

                if (searchFilter.ProcessTypeID > 0) query = query.Where(o => o.ProcessTypeID == searchFilter.ProcessTypeID);

                if (!string.IsNullOrEmpty(searchFilter.sStartDate) && !string.IsNullOrEmpty(searchFilter.sEndDate))
                    query = query.Where(o => o.RequestDate >= searchFilter.StartDate);

                if (!string.IsNullOrEmpty(searchFilter.SearchInWord))
                {
                    string keyword = searchFilter.SearchInWord.ToLower(); // Convert to lowercase for case-insensitive search

                    query = query.Where(o =>
                        o.StockRequestID.ToString().Contains(keyword) ||
                        o.ProcessTypeName != null && o.ProcessTypeName.ToLower().Contains(keyword) ||
                          o.RequestedByName != null && o.RequestedByName.ToLower().Contains(keyword) ||
                          o.RequestNo != null && o.RequestNo.ToLower().Contains(keyword)
                    );
                }
                //return query.Where(w => w.StatusID == 2 || w.StatusID==4).OrderBy(o => o.RequestNo).ToList();

                _auditLogger.SaveActionLog("VStockRequest", ActionType.ListData, null, searchFilter,null, "PendingApprovalServiceRepository.GetProductionInwardByFilter()");
                //Modified on 2025.03.27
                return query.Where(w => w.ProcessTypeID == 1 && (w.StatusID == 2 || w.StatusID == 4)  //Hydraulic Pressure
                                   ).OrderBy(o => o.RequestNo).ToList(); //Barcode Stickering
            }
            catch (Exception ex)
            {
                _errorLoggerService.LogException(ex, searchFilter, "PendingApprovalServiceRepository.GetProductionInwardByFilter()");
                return null;
            }
        }

    }
}
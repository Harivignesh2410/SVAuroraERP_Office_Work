namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Production
{
    public class StockRequestServiceRepository : IStockRequestServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IStockRequestTransServiceRepository _stocktransRepository;
        private readonly ILogger<IStockRequestServiceRepository> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public StockRequestServiceRepository(SVAuroraERPDbContext dbcontext, ILogger<IStockRequestServiceRepository> logger,
            IStockRequestTransServiceRepository stocktransRepository, IAuditLogger auditLogger, IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _stocktransRepository = stocktransRepository;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public List<VStockRequest> GetStockRequest()
        {
            try
            {
                var resultdata = _dbcontext.VStockRequest.OrderBy(o => o.RequestNo).ToList();

                _auditLogger.SaveActionLog("VStockRequest", ActionType.ListData, null, null, null, "StockRequestServiceRepository.GetStockRequest()");
                return resultdata;
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, null, "StockRequestServiceRepository.GetStockRequest()");
                return null;
            }
        }

        public VStockRequest GetByID(int ID)
        {
            try
            {
                var resultdata = _dbcontext.VStockRequest.FirstOrDefault(w => w.StockRequestID == ID);
                resultdata.VStockRequestTrans = _stocktransRepository.GetStockRequestTransByID(ID);

                _auditLogger.SaveActionLog("VStockRequest", ActionType.ListData, null, ID, null, "StockRequestServiceRepository.GetByID()");
                return resultdata;
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, ID, "StockRequestServiceRepository.GetByID()");
                return null;
            }
        }
        public Tuple<bool, string?> Save(StockRequest request)
        {
            bool IsSuccess = false;
            string? RequestNo = null;
            Tuple<int, string?> Result = null;

            try
            {
                Result = SaveStockRequest(request);

                foreach (var stockrequesttrans in request.StockRequestTrans)
                {
                    stockrequesttrans.StockRequestID = Result.Item1;
                    _stocktransRepository.AddStockRequestTrans(stockrequesttrans);
                }

                IsSuccess = true;
                _auditLogger.SaveActionLog("StockRequest", ActionType.Insert, request.StockRequestID.ToString(), request, null, "StockRequestServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, request, "StockRequestServiceRepository.Save()");
                IsSuccess = false;
            }
             return Tuple.Create(IsSuccess, Result.Item2);
        }

        public Tuple<bool, bool> Update(StockRequest request)
        {
            bool IsSuccess = false;
            bool doesItemExist = false;
            try
            {
                UpdateStockRequest(request);
                foreach (var stockrequesttrans in request.StockRequestTrans.Where(w => w.StockRequestTransID == 0).ToList())
                {
                    stockrequesttrans.StockRequestID = request.StockRequestID;
                    _stocktransRepository.AddStockRequestTrans(stockrequesttrans);
                }

                IsSuccess = true;
                doesItemExist = true;
                _auditLogger.SaveActionLog("StockRequest", ActionType.Update, request.StockRequestID.ToString(), request, null, "StockRequestServiceRepository.Update()");
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, request, "StockRequestServiceRepository.Update()");
                IsSuccess = false;
            }
             return Tuple.Create(IsSuccess, doesItemExist);
        }
        public Tuple<bool, bool> Delete(int StockRequestID, int UserID)
        {
            bool IsSuccess = false;
            bool doesItemExist = false;

            try
            {
                DeleteStockReport(StockRequestID, UserID);
                var stockRequestTrans = _dbcontext.StockRequestTrans.Where(w => w.StockRequestID == StockRequestID).ToList();

                foreach (var stockrequest in stockRequestTrans)
                {
                    _stocktransRepository.DeleteStockRequestTrans(stockrequest.StockRequestID);
                }
                IsSuccess = true;
                doesItemExist = true;
                _auditLogger.SaveActionLog("StockRequest", ActionType.Delete, null, StockRequestID, null, "StockRequestServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, StockRequestID, "StockRequestServiceRepository.Delete()");
                IsSuccess = false;
            }
             return Tuple.Create(IsSuccess, doesItemExist);
        }
        public List<BatchStock> GetBatchStockByFilter(BatchStockFilter request)
        {
            try
            {
                var query = _dbcontext.VBatchStock.AsQueryable();
                if (request.ProcessTypeID > 0)
                {
                    // Step 1: Get the list of ComponentTypeIDs
                    var componentIds = _dbcontext.VProductionConfiguration
                    .Where(w => w.ProcessTypeID == request.ProcessTypeID)
                    .Select(s => s.ComponentTypeID)
                    .Distinct()
                    .ToList();
                    query = query.Where(w => componentIds.Contains(w.ComponentTypeID));
                }

                if (request.SizeID > 0) query = query.Where(w => w.SizeID == request.SizeID);

                _auditLogger.SaveActionLog("VBatchStock", ActionType.ListData, null, request, null, "StockRequestServiceRepository.GetBatchStockByFilter()");
                return query.Where(w => w.StatusID < 3 && w.ProbableProductionQuantity >= 1).ToList();
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, request, "StockRequestServiceRepository.GetBatchStockByFilter()");
                return null;
            }
        }

        private Tuple<int, string> SaveStockRequest(StockRequest request)
        {
            int id = 0;
            string stockRequestData = string.Empty;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.INSERTSTOCKREQUEST;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var pkStockRequestIDParam = command.CreateParameter();
                        pkStockRequestIDParam.ParameterName = "@PK_StockRequestID";
                        pkStockRequestIDParam.Direction = System.Data.ParameterDirection.Output;
                        pkStockRequestIDParam.DbType = System.Data.DbType.Int32;
                        pkStockRequestIDParam.Value = 0;

                        var RequestDateParam = command.CreateParameter();
                        RequestDateParam.ParameterName = "@RequestDate";
                        RequestDateParam.Value = request.RequestDate;

                        var RequestedByParam = command.CreateParameter();
                        RequestedByParam.ParameterName = "@RequestedBy";
                        RequestedByParam.Value = request.RequestedBy;

                        var fkProcessTypeIDParam = command.CreateParameter();
                        fkProcessTypeIDParam.ParameterName = "@FK_ProcessTypeID";
                        fkProcessTypeIDParam.Value = request.ProcessTypeID;

                        var lastUpdatedByParam = command.CreateParameter();
                        lastUpdatedByParam.ParameterName = "@LastUpdatedBy";
                        lastUpdatedByParam.Value = request.LastUpdatedBy;

                        command.Parameters.Add(pkStockRequestIDParam);
                        command.Parameters.Add(RequestDateParam);
                        command.Parameters.Add(RequestedByParam);
                        command.Parameters.Add(fkProcessTypeIDParam);
                        command.Parameters.Add(lastUpdatedByParam);

                        command.ExecuteNonQuery();
                        id = (int)pkStockRequestIDParam.Value;
                        stockRequestData = _dbcontext.StockRequest
                                                    .Where(w => w.StockRequestID == id)
                                                    .Select(w => w.RequestNo)
                                                    .FirstOrDefault();
                    }
                }
                _auditLogger.SaveActionLog("StockRequest", ActionType.Insert, null, request, null, "StockRequestServiceRepository.SaveStockRequest()");
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, request, "StockRequestServiceRepository.SaveStockRequest()");
            }

            return Tuple.Create(id, stockRequestData);
        }
        private int UpdateStockRequest(StockRequest request)
        {
            int id = 0;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.UPDATESTOCKREQUEST;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var pkStockRequestIDParam = command.CreateParameter();
                        pkStockRequestIDParam.ParameterName = "@PK_StockRequestID";
                        pkStockRequestIDParam.Value = request.StockRequestID;

                        var RequestDateParam = command.CreateParameter();
                        RequestDateParam.ParameterName = "@RequestDate";
                        RequestDateParam.Value = request.RequestDate;

                        var RequestedByParam = command.CreateParameter();
                        RequestedByParam.ParameterName = "@RequestedBy";
                        RequestedByParam.Value = request.RequestedBy;

                        var fkProcessTypeIDParam = command.CreateParameter();
                        fkProcessTypeIDParam.ParameterName = "@FK_ProcessTypeID";
                        fkProcessTypeIDParam.Value = request.ProcessTypeID;

                        var lastUpdatedByParam = command.CreateParameter();
                        lastUpdatedByParam.ParameterName = "@LastUpdatedBy";
                        lastUpdatedByParam.Value = request.LastUpdatedBy;

                        command.Parameters.Add(pkStockRequestIDParam);
                        command.Parameters.Add(RequestDateParam);
                        command.Parameters.Add(RequestedByParam);
                        command.Parameters.Add(fkProcessTypeIDParam);
                        command.Parameters.Add(lastUpdatedByParam);

                        id = command.ExecuteNonQuery();
                    }
                }
                _auditLogger.SaveActionLog("StockRequest", ActionType.Update, null, request, null, "StockRequestServiceRepository.UpdateStockRequest()");
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, request, "StockRequestServiceRepository.UpdateStockRequest()");
            }
            return id;
        }
        private int DeleteStockReport(int StockRequestID, int LastUpdatedBy)
        {
            int id = 0;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.DELETESTOCKREQUEST;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var receiptIdParam = command.CreateParameter();
                        receiptIdParam.ParameterName = "@PK_StockRequestID";
                        receiptIdParam.Value = StockRequestID;
                        receiptIdParam.DbType = System.Data.DbType.Int32;

                        var lastUpdatedByParam = command.CreateParameter();
                        lastUpdatedByParam.ParameterName = "@LastUpdatedBy";
                        lastUpdatedByParam.Value = LastUpdatedBy;

                        command.Parameters.Add(receiptIdParam);
                        command.Parameters.Add(lastUpdatedByParam);


                        id = command.ExecuteNonQuery();

                    }
                    _auditLogger.SaveActionLog("StockRequest", ActionType.Delete, null, StockRequestID, null, "StockRequestServiceRepository.DeleteStockReport()");
                }
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, StockRequestID, "StockRequestServiceRepository.DeleteStockReport()");
            }
            return id;
        }
    }
}
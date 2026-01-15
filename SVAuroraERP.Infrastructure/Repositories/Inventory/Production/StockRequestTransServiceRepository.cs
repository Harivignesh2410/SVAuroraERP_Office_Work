namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Production
{
    public class StockRequestTransServiceRepository : IStockRequestTransServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;

        public StockRequestTransServiceRepository(SVAuroraERPDbContext dbContext, IAuditLogger auditLogger, IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbContext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public List<VStockRequestTrans> GetStockRequestTransByID(int StockRequestID)
        {
            try
            {
                var resultdata = _dbcontext.VStockRequestTrans.Where(w => w.StockRequestID == StockRequestID).ToList();
                _auditLogger.SaveActionLog("VStockRequestTrans", ActionType.ListData, null, StockRequestID,null, "StockRequestTransServiceRepository.GetStockRequestTransByID()");
                return resultdata;
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, StockRequestID, "StockRequestTransServiceRepository.GetStockRequestTransByID()");
                return null;
            }
        }
        public Tuple<bool, bool> SaveStockRequestTrans(List<StockRequestTrans> request)
        {
            bool IsSuccess = false;
            bool doesSupplierExist = false;
            try
            {
                int SuccessCount = 0;
                if (request == null || request.Count == 0)
                {
                    return Tuple.Create(IsSuccess, doesSupplierExist);
                }

                foreach (var stockRequestTrans in request)
                {
                    if (stockRequestTrans.StatusFlag == "I") //Insert
                    {
                        int id = AddStockRequestTrans(stockRequestTrans);
                        if (id != 0) SuccessCount++;
                    }
                    else if (stockRequestTrans.StatusFlag == "D") //Delete
                    {
                        int id = DeleteStockRequestTrans(stockRequestTrans.StockRequestTransID);
                        if (id != 0) SuccessCount++;
                    }
                }

                if (SuccessCount > 0)
                {
                    IsSuccess = true;
                    doesSupplierExist = true;
                }
                _auditLogger.SaveActionLog("StockRequestTrans", ActionType.Insert, null, request,null, "StockRequestTransServiceRepository.SaveStockRequestTrans()");
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, request, "StockRequestTransServiceRepository.SaveStockRequestTrans()");
            }
            return Tuple.Create(IsSuccess, doesSupplierExist); ;
        }
        public int AddStockRequestTrans(StockRequestTrans request)
        {
            int id = 0;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.INSERTSTOCKREQUESTTRANS;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var fkStockRequestIDParam = command.CreateParameter();
                        fkStockRequestIDParam.ParameterName = "@FK_StockRequestID";
                        fkStockRequestIDParam.Value = request.StockRequestID;

                        var fkBatchStockIDParam = command.CreateParameter();
                        fkBatchStockIDParam.ParameterName = "@FK_BatchStockID";
                        fkBatchStockIDParam.Value = request.BatchStockID;

                        var quantityParam = command.CreateParameter();
                        quantityParam.ParameterName = "@Quantity";
                        quantityParam.Value = request.Quantity;

                        command.Parameters.Add(fkStockRequestIDParam);
                        command.Parameters.Add(fkBatchStockIDParam);
                        command.Parameters.Add(quantityParam);

                        id = command.ExecuteNonQuery();
                    }
                }
                _auditLogger.SaveActionLog("StockRequestTrans", ActionType.Insert, null, request, null, "StockRequestTransServiceRepository.AddStockRequestTrans()");
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, request, "StockRequestTransServiceRepository.AddStockRequestTrans()");
            }

            return id;
        }
        public int DeleteStockRequestTrans(int StockRequestTransID)
        {
            int id = 0;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.DELETESTOCKREQUESTTRANS;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var pkStockRequestTransIDParam = command.CreateParameter();
                        pkStockRequestTransIDParam.ParameterName = "@PK_StockRequestTransID";
                        pkStockRequestTransIDParam.Value = StockRequestTransID;

                        command.Parameters.Add(pkStockRequestTransIDParam);

                        id = command.ExecuteNonQuery();
                    }
                }
                _auditLogger.SaveActionLog("StockRequestTrans", ActionType.Delete, null, StockRequestTransID,null, "StockRequestTransServiceRepository.DeleteStockRequestTrans()");
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, StockRequestTransID, "StockRequestTransServiceRepository.DeleteStockRequestTrans()");
            }

            return id;
        }
    }
}
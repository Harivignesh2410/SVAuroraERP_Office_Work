namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Production
{
    public class HydrolicPressureServiceRepository : IHydrolicPressureServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<PendingApprovalServiceRepository> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;

        public HydrolicPressureServiceRepository(SVAuroraERPDbContext dbcontext, ILogger<PendingApprovalServiceRepository> logger, 
                                                IAuditLogger auditLogger, IErrorLoggerService errorLoggerService)

        {
            _dbcontext = dbcontext;
            _logger = logger;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }


        public List<VStockRequest> GetStockRequestList(int id)
        {
            try
            {
                List<VStockRequest> resultdata = null;
                if (id == 1)
                {
                     resultdata = _dbcontext.VStockRequest.Where(w => w.ProcessTypeID == 1 && w.StatusID == 2   //Approved 
                                       ).OrderBy(o => o.RequestNo).ToList();
                }
                else if (id==2)
                {
                     resultdata = _dbcontext.VStockRequest.Where(w => w.ProcessTypeID == 1 && w.StatusID == 4   //In Progress 
                                   ).OrderBy(o => o.RequestNo).ToList();
                }
                else if (id==3)
                {
                     resultdata = _dbcontext.VStockRequest.Where(w => w.ProcessTypeID == 1 && w.StatusID == 5   // Completed 
                                   ).OrderBy(o => o.RequestNo).ToList();
                }
                _auditLogger.SaveActionLog("VStockRequest", ActionType.ListData, null, id,null, "HydrolicPressureServiceRepository.GetStockRequestList()");
                return resultdata;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, id, "HydrolicPressureServiceRepository.GetStockRequestList()");
                return null;
            }
        }
        public Tuple<bool, bool> SaveHydrolicPressure(HydrolicPressure request)
        {
            int id = 0;
            bool IsSuccess = false;
            bool IsError = false;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.INSERTHYDROLICPRESSURE;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var pkHydrolicPressureIDParam = command.CreateParameter();
                        pkHydrolicPressureIDParam.ParameterName = "@PK_HydrolicPressureID";
                        pkHydrolicPressureIDParam.Direction = System.Data.ParameterDirection.Output;
                        pkHydrolicPressureIDParam.DbType = System.Data.DbType.Int32;
                        pkHydrolicPressureIDParam.Value = 0;

                        var fkStockRequestIDParam = command.CreateParameter();
                        fkStockRequestIDParam.ParameterName = "@FK_StockRequestID";
                        fkStockRequestIDParam.Value = request.StockRequestID;

                        var fkItemIDParam = command.CreateParameter();
                        fkItemIDParam.ParameterName = "@FK_ItemID";
                        fkItemIDParam.Value = request.ItemID;

                        var fkRackLocationIDParam = command.CreateParameter();
                        fkRackLocationIDParam.ParameterName = "@FK_RackLocationID";
                        fkRackLocationIDParam.Value = request.RackLocationID;

                        var fkOperatorIDParam = command.CreateParameter();
                        fkOperatorIDParam.ParameterName = "@FK_OperatorID";
                        fkOperatorIDParam.Value = request.OperatorID;

                        var StartTimeParam = command.CreateParameter();
                        StartTimeParam.ParameterName = "@StartTime";
                        StartTimeParam.Value = request.StartTime;

                        var EndTimeParam = command.CreateParameter();
                        EndTimeParam.ParameterName = "@EndTime";
                        EndTimeParam.Value = request.EndTime;

                        var ProductionDateParam = command.CreateParameter();
                        ProductionDateParam.ParameterName = "@ProductionDate";
                        ProductionDateParam.Value = request.ProductionDate;

                        var ProductionQtyParam = command.CreateParameter();
                        ProductionQtyParam.ParameterName = "@ProductionQty";
                        ProductionQtyParam.Value = request.ProductionQty;

                        var WastageQtyParam = command.CreateParameter();
                        WastageQtyParam.ParameterName = "@WastageQty";
                        WastageQtyParam.Value = request.WastageQty;

                        var OtherWastageQtyParam = command.CreateParameter();
                        OtherWastageQtyParam.ParameterName = "@OtherWastageQty";
                        OtherWastageQtyParam.Value = request.OtherWastageQty;

                        var LastUpdatedByParam = command.CreateParameter();
                        LastUpdatedByParam.ParameterName = "@LastUpdatedBy";
                        LastUpdatedByParam.Value = request.LastUpdatedBy;



                        command.Parameters.Add(pkHydrolicPressureIDParam);
                        command.Parameters.Add(fkStockRequestIDParam);
                        command.Parameters.Add(fkItemIDParam);
                        command.Parameters.Add(fkRackLocationIDParam);
                        command.Parameters.Add(fkOperatorIDParam);
                        command.Parameters.Add(StartTimeParam);
                        command.Parameters.Add(EndTimeParam);
                        command.Parameters.Add(ProductionDateParam);
                        command.Parameters.Add(ProductionQtyParam);
                        command.Parameters.Add(WastageQtyParam);
                        command.Parameters.Add(OtherWastageQtyParam);
                        command.Parameters.Add(LastUpdatedByParam);


                        command.ExecuteNonQuery();
                        id = (int)pkHydrolicPressureIDParam.Value;

                    }
                }
                foreach (var HydrolicConsumption in request.HydrolicConsumption)
                {
                    HydrolicConsumption.HydrolicPressureID = id;
                    if (SaveHydrolicConsumption(HydrolicConsumption)) IsSuccess = true;
                    else IsError = true;
                }
                _auditLogger.SaveActionLog("HydrolicPressure", ActionType.Insert, request.HydrolicPressureID.ToString(), request,null, "HydrolicPressureServiceRepository.SaveHydrolicPressure()");
                return Tuple.Create(IsSuccess, IsError);
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, request, "HydrolicPressureServiceRepository.SaveHydrolicPressure()");
                return null;
            }
        }
        public Tuple<bool, bool> UpdateHydrolicPressure(HydrolicPressure request)
        {
            bool IsSuccess = false;
            bool IsError = false;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.UPDATEHYDROLICPRESSURE;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var pkHydrolicPressureIDParam = command.CreateParameter();
                        pkHydrolicPressureIDParam.ParameterName = "@PK_HydrolicPressureID";
                        pkHydrolicPressureIDParam.Value = request.HydrolicPressureID;

                        var fkStockRequestIDParam = command.CreateParameter();
                        fkStockRequestIDParam.ParameterName = "@FK_StockRequestID";
                        fkStockRequestIDParam.Value = request.StockRequestID;

                        var fkItemIDParam = command.CreateParameter();
                        fkItemIDParam.ParameterName = "@FK_ItemID";
                        fkItemIDParam.Value = request.ItemID;

                        var fkRackLocationIDParam = command.CreateParameter();
                        fkRackLocationIDParam.ParameterName = "@FK_RackLocationID";
                        fkRackLocationIDParam.Value = request.RackLocationID;

                        var fkOperatorIDParam = command.CreateParameter();
                        fkOperatorIDParam.ParameterName = "@FK_OperatorID";
                        fkOperatorIDParam.Value = request.OperatorID;

                        var StartTimeParam = command.CreateParameter();
                        StartTimeParam.ParameterName = "@StartTime";
                        StartTimeParam.Value = request.StartTime;

                        var EndTimeParam = command.CreateParameter();
                        EndTimeParam.ParameterName = "@EndTime";
                        EndTimeParam.Value = request.EndTime;

                        var ProductionDateParam = command.CreateParameter();
                        ProductionDateParam.ParameterName = "@ProductionDate";
                        ProductionDateParam.Value = request.ProductionDate;

                        var ProductionQtyParam = command.CreateParameter();
                        ProductionQtyParam.ParameterName = "@ProductionQty";
                        ProductionQtyParam.Value = request.ProductionQty;

                        var WastageQtyParam = command.CreateParameter();
                        WastageQtyParam.ParameterName = "@WastageQty";
                        WastageQtyParam.Value = request.WastageQty;

                        var OtherWastageQtyParam = command.CreateParameter();
                        OtherWastageQtyParam.ParameterName = "@OtherWastageQty";
                        OtherWastageQtyParam.Value = request.OtherWastageQty;

                        var LastUpdatedByParam = command.CreateParameter();
                        LastUpdatedByParam.ParameterName = "@LastUpdatedBy";
                        LastUpdatedByParam.Value = request.LastUpdatedBy;



                        command.Parameters.Add(pkHydrolicPressureIDParam);
                        command.Parameters.Add(fkStockRequestIDParam);
                        command.Parameters.Add(fkItemIDParam);
                        command.Parameters.Add(fkRackLocationIDParam);
                        command.Parameters.Add(fkOperatorIDParam);
                        command.Parameters.Add(StartTimeParam);
                        command.Parameters.Add(EndTimeParam);
                        command.Parameters.Add(ProductionDateParam);
                        command.Parameters.Add(ProductionQtyParam);
                        command.Parameters.Add(WastageQtyParam);
                        command.Parameters.Add(OtherWastageQtyParam);
                        command.Parameters.Add(LastUpdatedByParam);

                        command.ExecuteNonQuery();
                        IsSuccess = true;
                    }
                }
                foreach (var HydrolicConsumption in request.HydrolicConsumption)
                {
                    HydrolicConsumption.HydrolicPressureID = request.HydrolicPressureID;
                    if (SaveHydrolicConsumption(HydrolicConsumption)) IsSuccess = true;
                    else IsError = true;
                }

                _auditLogger.SaveActionLog("HydrolicPressure", ActionType.Update, request.HydrolicPressureID.ToString(), request,null, "HydrolicPressureServiceRepository.UpdateHydrolicPressure()");
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, request, "HydrolicPressureServiceRepository.UpdateHydrolicPressure()");
            }
            return Tuple.Create(IsSuccess, IsError);
        }
        public bool SaveHydrolicConsumption(HydrolicConsumption request)
        {
            bool IsSuccess = false;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.INSERTHYDROLICCONSUMPTION;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var hydrolicpressureIDParam = command.CreateParameter();
                        hydrolicpressureIDParam.ParameterName = "@FK_HydrolicPressureID";
                        hydrolicpressureIDParam.Value = request.HydrolicPressureID;

                        var StockRequestTransIDParam = command.CreateParameter();
                        StockRequestTransIDParam.ParameterName = "@FK_StockRequestTransID";
                        StockRequestTransIDParam.Value = request.StockRequestTransID;

                        var ActualConsumedQtyParam = command.CreateParameter();
                        ActualConsumedQtyParam.ParameterName = "@ActualConsumedQty";
                        ActualConsumedQtyParam.Value = request.ActualConsumedQty;

                        var WastageQtyParam = command.CreateParameter();
                        WastageQtyParam.ParameterName = "@WastageQty";
                        WastageQtyParam.Value = request.WastageQty;

                        var WastagePercentageParam = command.CreateParameter();
                        WastagePercentageParam.ParameterName = "@WastagePercentage";
                        WastagePercentageParam.Value = request.WastagePercentage;

                        var BalanceQtyParam = command.CreateParameter();
                        BalanceQtyParam.ParameterName = "@BalanceQty";
                        BalanceQtyParam.Value = request.BalanceQty;

                        command.Parameters.Add(hydrolicpressureIDParam);
                        command.Parameters.Add(StockRequestTransIDParam);
                        command.Parameters.Add(ActualConsumedQtyParam);
                        command.Parameters.Add(WastageQtyParam);
                        command.Parameters.Add(WastagePercentageParam);
                        command.Parameters.Add(BalanceQtyParam);

                        command.ExecuteNonQuery();
                        IsSuccess = true;
                    }
                }
                _auditLogger.SaveActionLog("HydrolicConsumption", ActionType.Insert, request.HydrolicConsumptionID.ToString(), request,null,"HydrolicPressureServiceRepository.SaveHydrolicConsumption()");
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, request, "HydrolicPressureServiceRepository.SaveHydrolicConsumption()");
            }
            return IsSuccess;
        }
        public bool UpdateHydrolicConsumption(HydrolicConsumption request)
        {
            bool IsSuccess = false;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.UPDATEHYDROLICCONSUMPTION;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var pkHydrolicConsumptionParams = command.CreateParameter();
                        pkHydrolicConsumptionParams.ParameterName = "@PK_HydrolicConsumptionID";
                        pkHydrolicConsumptionParams.Value = request.HydrolicConsumptionID;

                        var hydrolicpressureIDParam = command.CreateParameter();
                        hydrolicpressureIDParam.ParameterName = "@FK_HydrolicPressureID";
                        hydrolicpressureIDParam.Value = request.HydrolicPressureID;

                        var StockRequestTransIDParam = command.CreateParameter();
                        StockRequestTransIDParam.ParameterName = "@FK_StockRequestTransID";
                        StockRequestTransIDParam.Value = request.StockRequestTransID;

                        var ActualConsumedQtyParam = command.CreateParameter();
                        ActualConsumedQtyParam.ParameterName = "@ActualConsumedQty";
                        ActualConsumedQtyParam.Value = request.ActualConsumedQty;

                        var WastageQtyParam = command.CreateParameter();
                        WastageQtyParam.ParameterName = "@WastageQty";
                        WastageQtyParam.Value = request.WastageQty;

                        var WastagePercentageParam = command.CreateParameter();
                        WastagePercentageParam.ParameterName = "@WastagePercentage";
                        WastagePercentageParam.Value = request.WastagePercentage;

                        var BalanceQtyParam = command.CreateParameter();
                        BalanceQtyParam.ParameterName = "@BalanceQty";
                        BalanceQtyParam.Value = request.BalanceQty;

                        command.Parameters.Add(pkHydrolicConsumptionParams);
                        command.Parameters.Add(hydrolicpressureIDParam);
                        command.Parameters.Add(StockRequestTransIDParam);
                        command.Parameters.Add(ActualConsumedQtyParam);
                        command.Parameters.Add(WastageQtyParam);
                        command.Parameters.Add(WastagePercentageParam);
                        command.Parameters.Add(BalanceQtyParam);

                        command.ExecuteNonQuery();
                        IsSuccess = true;
                    }
                }
                _auditLogger.SaveActionLog("HydrolicConsumption", ActionType.Update, request.HydrolicConsumptionID.ToString(), request, null,"HydrolicPressureServiceRepository.UpdateHydrolicConsumption()");
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, request, "HydrolicPressureServiceRepository.UpdateHydrolicConsumption()");
             
            }
            return IsSuccess;
        }
        public FullHydraulicDataResult GetHydraulicDetails(int stockRequestID)
        {
            var result = new FullHydraulicDataResult();
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                using (var command = new SqlCommand("GetHydrolicPressureDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StockRequestID", stockRequestID);

                    var adapter = new SqlDataAdapter(command);
                    var dataSet = new DataSet();

                    adapter.Fill(dataSet); // Fills all result sets

                    // Convert each table into List<T> or DataTable
                    if (dataSet.Tables.Count > 0) result.StockRequest = dataSet.Tables[0];
                    if (dataSet.Tables.Count > 1) result.StockRequestTrans = dataSet.Tables[1];
                    if (dataSet.Tables.Count > 2) result.HydrolicPressure = dataSet.Tables[2];
                    if (dataSet.Tables.Count > 3) result.HydrolicConsumption = dataSet.Tables[3];
                }
                _auditLogger.SaveActionLog("FullHydraulicDataResult", ActionType.ListData, null, stockRequestID,null, "HydrolicPressureServiceRepository.GetHydraulicDetails()");
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, stockRequestID, "HydrolicPressureServiceRepository.GetHydraulicDetails()");
                return null;
            }
            return result;
        }
        public HydraulicDataResponse GetHydraulicDetailsAsync(int stockRequestId)
        {
            try
            {
                var dataResult = GetHydraulicDetails(stockRequestId);

                _auditLogger.SaveActionLog("HydraulicDataResponse", ActionType.ListData, null, stockRequestId, null,"HydrolicPressureServiceRepository.GetHydraulicDetailsAsync()");
                return new HydraulicDataResponse
                {
                    StockRequest = dataResult.StockRequest?.ToList<VStockRequest>() ?? new List<VStockRequest>(),
                    StockRequestTrans = dataResult.StockRequestTrans?.ToList<VStockRequestTrans>() ?? new List<VStockRequestTrans>(),
                    HydrolicPressure = dataResult.HydrolicPressure?.ToList<VHydrolicPressure>() ?? new List<VHydrolicPressure>(),
                    HydrolicConsumption = dataResult.HydrolicConsumption?.ToList<HydrolicConsumption>() ?? new List<HydrolicConsumption>()
                };
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, stockRequestId, "HydrolicPressureServiceRepository.GetHydraulicDetailsAsync()");
                return null;
            }
        }
        public Tuple<bool, bool> DeleteHydrolicPressure(int HydrolicPressureID)
        {
            bool IsSuccess = false;
            bool IsError = false;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.DELETEHYDROLICPRESSURE;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var hydrolicpressureIDParam = command.CreateParameter();
                        hydrolicpressureIDParam.ParameterName = "@PK_HydrolicPressureID";
                        hydrolicpressureIDParam.Value = HydrolicPressureID;

                        command.Parameters.Add(hydrolicpressureIDParam);

                        command.ExecuteNonQuery();
                        IsSuccess = true;
                    }
                }
                _auditLogger.SaveActionLog("HydraulicPressure", ActionType.Delete, null,HydrolicPressureID,null, "HydrolicPressureServiceRepository.DeleteHydrolicPressure()");
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, HydrolicPressureID, "HydrolicPressureServiceRepository.DeleteHydrolicPressure()");
                return null;
            }
            return Tuple.Create(IsSuccess, IsError);
        }
        public Tuple<bool, bool> CompleteHydrolicPressure(int StockRequestID)
        {
            bool IsSuccess = false;
            bool IsError = false;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.COMPLETEHYDROLICPRESSURE;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var StockRequestIDParam = command.CreateParameter();
                        StockRequestIDParam.ParameterName = "@PK_StockRequestID";
                        StockRequestIDParam.Value = StockRequestID;

                        command.Parameters.Add(StockRequestIDParam);

                        command.ExecuteNonQuery();
                        IsSuccess = true;
                    }
                }
                _auditLogger.SaveActionLog("HydraulicPressure", ActionType.Insert, StockRequestID.ToString(), StockRequestID,null, "HydrolicPressureServiceRepository.CompleteHydrolicPressure()");
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, StockRequestID, "HydrolicPressureServiceRepository.CompleteHydrolicPressure()");
                return null;
            }
            return Tuple.Create(IsSuccess, IsError);
        }

        public HydrolicPressureBatchStock GetHydrolicPressureByID(int BatchStockID)
        {
            try
            {
                var resultdata = _dbcontext.HydrolicPressureBatchStock.FirstOrDefault(w => w.BatchStockID == BatchStockID);
                _auditLogger.SaveActionLog("HydrolicPressureBatchStock", ActionType.ListData, null, BatchStockID,null, "HydrolicPressureServiceRepository.GetHydrolicPressureByID()");

                return resultdata;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, BatchStockID, "HydrolicPressureServiceRepository.GetHydrolicPressureByID()");
                return null;
            }
        }

    }
}

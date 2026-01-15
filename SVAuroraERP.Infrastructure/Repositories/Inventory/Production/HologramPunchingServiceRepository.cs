namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Production
{
    public class HologramPunchingServiceRepository : IHologramPunchingServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<HologramPunchingServiceRepository> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public HologramPunchingServiceRepository(SVAuroraERPDbContext dbcontext, ILogger<HologramPunchingServiceRepository> logger, IAuditLogger auditLogger,
                                                 IErrorLoggerService errorLoggerService,
                                                 IErrorLoggerService _errorLoggerService)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public Tuple<bool, bool, bool> SaveHologramPunching(HologramPunching request)
        {
            int id = 0;
            bool IsSuccess = false;
            bool IsError = false;
            bool ProductionCompleted = false;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.INSERTHOLOGRAMPUNCHING;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var pkHologramPunchingIDParam = command.CreateParameter();
                        pkHologramPunchingIDParam.ParameterName = "@PK_HologramPunchingID";
                        pkHologramPunchingIDParam.Direction = System.Data.ParameterDirection.Output;
                        pkHologramPunchingIDParam.DbType = System.Data.DbType.Int32;
                        pkHologramPunchingIDParam.Value = 0;

                        var stockRequestIDParam = command.CreateParameter();
                        stockRequestIDParam.ParameterName = "@Fk_StockRequestID";
                        stockRequestIDParam.Value = request.StockRequestID;

                        var fkinputbatchstockIDParam = command.CreateParameter();
                        fkinputbatchstockIDParam.ParameterName = "@FK_InputBatchStockID";
                        fkinputbatchstockIDParam.Value = request.InputBatchStockID;

                        var fkhologramplateIDParam = command.CreateParameter();
                        fkhologramplateIDParam.ParameterName = "@FK_HologramPlateID";
                        fkhologramplateIDParam.Value = request.HologramPlateID;

                        var fkracklocationIDParam = command.CreateParameter();
                        fkracklocationIDParam.ParameterName = "@FK_RackLocationID";
                        fkracklocationIDParam.Value = request.RackLocationID;

                        var machineIDParam = command.CreateParameter();
                        machineIDParam.ParameterName = "@FK_MachineID";
                        machineIDParam.Value = request.MachineID;

                        var operatorIDParam = command.CreateParameter();
                        operatorIDParam.ParameterName = "@FK_OperatorID";
                        operatorIDParam.Value = request.OperatorID;

                        var starttimeParam = command.CreateParameter();
                        starttimeParam.ParameterName = "@StartTime";
                        starttimeParam.Value = request.StartTime;

                        var endtimeParam = command.CreateParameter();
                        endtimeParam.ParameterName = "@EndTime";
                        endtimeParam.Value = request.EndTime;

                        var productionDateParam = command.CreateParameter();
                        productionDateParam.ParameterName = "@ProductionDate";
                        productionDateParam.Value = request.ProductionDate;

                        var hologramFinishedQtyParam = command.CreateParameter();
                        hologramFinishedQtyParam.ParameterName = "@HologramFinishedQty";
                        hologramFinishedQtyParam.Value = request.HologramFinishedQty;

                        var rejectedPlateQtyParam = command.CreateParameter();
                        rejectedPlateQtyParam.ParameterName = "@RejectedPlateQty";
                        rejectedPlateQtyParam.Value = request.RejectedPlateQty;

                        var hologramWastageQtyParam = command.CreateParameter();
                        hologramWastageQtyParam.ParameterName = "@HologramWastageQty";
                        hologramWastageQtyParam.Value = request.HologramWastageQty;

                        var lastUpdatedByParam = command.CreateParameter();
                        lastUpdatedByParam.ParameterName = "@LastUpdatedBy";
                        lastUpdatedByParam.Value = request.LastUpdatedBy;


                        command.Parameters.Add(pkHologramPunchingIDParam);
                        command.Parameters.Add(stockRequestIDParam);
                        command.Parameters.Add(fkinputbatchstockIDParam);
                        command.Parameters.Add(fkhologramplateIDParam);
                        command.Parameters.Add(fkracklocationIDParam);
                        command.Parameters.Add(machineIDParam);
                        command.Parameters.Add(operatorIDParam);
                        command.Parameters.Add(starttimeParam);
                        command.Parameters.Add(endtimeParam);
                        command.Parameters.Add(productionDateParam);
                        command.Parameters.Add(hologramFinishedQtyParam);
                        command.Parameters.Add(rejectedPlateQtyParam);
                        command.Parameters.Add(hologramWastageQtyParam);
                        command.Parameters.Add(lastUpdatedByParam);


                        command.ExecuteNonQuery();
                        id = (int)pkHologramPunchingIDParam.Value;

                        foreach (var Hologramconsumption in request.HologramConsumption)
                        {
                            Hologramconsumption.HologramPunchingID = id;
                            var result = SaveHologramConsumption(Hologramconsumption);

                            if (result.Item1) IsSuccess = true;           // Call succeeded
                            if (!result.Item1) IsError = true;            // Call failed
                            if (result.Item2)
                            {
                                ProductionCompleted = true;
                            }
                        }
                    }
                }
                _auditLogger.SaveActionLog("HologramPunching", ActionType.Insert, request.HologramPunchingID.ToString(), request, null, "HologramPunchingServiceRepository.SaveHologramPunching()");
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, request, "HologramPunchingServiceRepository.SaveHologramPunching()");
            }
            return Tuple.Create(IsSuccess, IsError, ProductionCompleted);
        }
        public Tuple<bool, bool, bool> UpdateHologramPunching(HologramPunching request)
        {
            bool IsSuccess = false;
            bool IsError = false;
            bool ProductionCompleted = false;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.UPDATEHOLOGRAMPUNCHING;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var pkHologramPunchingIDParam = command.CreateParameter();
                        pkHologramPunchingIDParam.ParameterName = "@PK_HologramPunchingID";
                        pkHologramPunchingIDParam.Value = request.HologramPunchingID;

                        var stockRequestIDParam = command.CreateParameter();
                        stockRequestIDParam.ParameterName = "@Fk_StockRequestID";
                        stockRequestIDParam.Value = request.StockRequestID;

                        var fkinputbatchstockIDParam = command.CreateParameter();
                        fkinputbatchstockIDParam.ParameterName = "@FK_InputBatchStockID";
                        fkinputbatchstockIDParam.Value = request.InputBatchStockID;

                        var fkhologramplateIDParam = command.CreateParameter();
                        fkhologramplateIDParam.ParameterName = "@FK_HologramPlateID";
                        fkhologramplateIDParam.Value = request.HologramPlateID;

                        var fkracklocationIDParam = command.CreateParameter();
                        fkracklocationIDParam.ParameterName = "@FK_RackLocationID";
                        fkracklocationIDParam.Value = request.RackLocationID;

                        var machineIDParam = command.CreateParameter();
                        machineIDParam.ParameterName = "@FK_MachineID";
                        machineIDParam.Value = request.MachineID;

                        var operatorIDParam = command.CreateParameter();
                        operatorIDParam.ParameterName = "@FK_OperatorID";
                        operatorIDParam.Value = request.OperatorID;

                        var starttimeParam = command.CreateParameter();
                        starttimeParam.ParameterName = "@StartTime";
                        starttimeParam.Value = request.StartTime;

                        var endtimeParam = command.CreateParameter();
                        endtimeParam.ParameterName = "@EndTime";
                        endtimeParam.Value = request.EndTime;

                        var productionDateParam = command.CreateParameter();
                        productionDateParam.ParameterName = "@ProductionDate";
                        productionDateParam.Value = request.ProductionDate;

                        var hologramFinishedQtyParam = command.CreateParameter();
                        hologramFinishedQtyParam.ParameterName = "@HologramFinishedQty";
                        hologramFinishedQtyParam.Value = request.HologramFinishedQty;

                        var rejectedPlateQtyParam = command.CreateParameter();
                        rejectedPlateQtyParam.ParameterName = "@RejectedPlateQty";
                        rejectedPlateQtyParam.Value = request.RejectedPlateQty;

                        var hologramWastageQtyParam = command.CreateParameter();
                        hologramWastageQtyParam.ParameterName = "@HologramWastageQty";
                        hologramWastageQtyParam.Value = request.HologramWastageQty;

                        var lastUpdatedByParam = command.CreateParameter();
                        lastUpdatedByParam.ParameterName = "@LastUpdatedBy";
                        lastUpdatedByParam.Value = request.LastUpdatedBy;


                        command.Parameters.Add(pkHologramPunchingIDParam);
                        command.Parameters.Add(stockRequestIDParam);
                        command.Parameters.Add(fkinputbatchstockIDParam);
                        command.Parameters.Add(fkhologramplateIDParam);
                        command.Parameters.Add(fkracklocationIDParam);
                        command.Parameters.Add(machineIDParam);
                        command.Parameters.Add(operatorIDParam);
                        command.Parameters.Add(starttimeParam);
                        command.Parameters.Add(endtimeParam);
                        command.Parameters.Add(productionDateParam);
                        command.Parameters.Add(hologramFinishedQtyParam);
                        command.Parameters.Add(rejectedPlateQtyParam);
                        command.Parameters.Add(hologramWastageQtyParam);
                        command.Parameters.Add(lastUpdatedByParam);

                        command.ExecuteNonQuery();
                        IsSuccess = true;
                    }
                }
                foreach (var Hologramconsumption in request.HologramConsumption)
                {
                    Hologramconsumption.HologramPunchingID = request.HologramPunchingID;
                    var result = SaveHologramConsumption(Hologramconsumption);

                    if (result.Item1) IsSuccess = true;           // Call succeeded
                    if (!result.Item1) IsError = true;            // Call failed
                    if (result.Item2)
                    {
                        ProductionCompleted = true;
                    }

                }
                _auditLogger.SaveActionLog("HologramPunching", ActionType.Update, request.HologramPunchingID.ToString(), request, null, "HologramPunchingServiceRepository.UpdateHologramPunching()");
            }

            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, request, "HologramPunchingServiceRepository.UpdateHologramPunching()");
            }
            return Tuple.Create(IsSuccess, IsError, ProductionCompleted);
        }
        public Tuple<bool, bool> SaveHologramConsumption(HologramConsumption request)
        {
            bool isCallSuccessful = false;
            bool isProductionCompleted = false;
            try
            {

                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.INSERTHOLOGRAMCONSUMPTION;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@FK_HologramPunchingID", request.HologramPunchingID);
                        command.Parameters.AddWithValue("@FK_BatchStockID", request.BatchStockID);
                        command.Parameters.AddWithValue("@ActualConsumedQty", request.ActualConsumedQty);
                        command.Parameters.AddWithValue("@WastageQty", request.WastageQty);
                        command.Parameters.AddWithValue("@WastagePercentage", request.WastagePercentage);
                        command.Parameters.AddWithValue("@BalanceQty", request.BalanceQty);

                        var resultFlagParam = command.CreateParameter();
                        resultFlagParam.ParameterName = "@ResultFlag";
                        resultFlagParam.SqlDbType = SqlDbType.Bit;
                        resultFlagParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultFlagParam);

                        command.ExecuteNonQuery();

                        isCallSuccessful = true;
                        isProductionCompleted = Convert.ToBoolean(resultFlagParam.Value);
                    }
                }
                _auditLogger.SaveActionLog("HologramConsumption", ActionType.Insert, request.HologramConsumptionID.ToString(), request, null, "HologramPunchingServiceRepository.SaveHologramConsumption()");
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, request, "HologramPunchingServiceRepository.SaveHologramConsumption()");
            }
            return Tuple.Create(isCallSuccessful, isProductionCompleted);
        }
        public List<VStockRequest> GetHologramPunchingList()
        {
            try
            {
                var resultdata = _dbcontext.VStockRequest.Where(w => w.StatusID == 2 && w.ProcessTypeID == 2).ToList();
                _auditLogger.SaveActionLog("VStockRequest", ActionType.ListData, null, null, null, "HologramPunchingServiceRepository.GetHologramPunchingList()");
                return resultdata;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "HologramPunchingServiceRepository.GetHologramPunchingList()");
                return new List<VStockRequest>();
            }
        }
        public HologramPunching GetHologramPunchingByID(int StockRequestID)
        {
            try
            {
                var resultdata = _dbcontext.HologramPunching.FirstOrDefault(w => w.StockRequestID == StockRequestID);
                _auditLogger.SaveActionLog("HologramPunching", ActionType.ListData, null, StockRequestID, null, "HologramPunchingServiceRepository.GetHologramPunchingByID()");
                return resultdata;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "HologramPunchingServiceRepository.GetHologramPunchingByID()");
                return new HologramPunching();
            }
        }

        public List<VStockRequest> GetStockRequestList(int id)
        {
            try
            {
                List<VStockRequest> resultdata = null;
                if (id == 1)
                {
                    resultdata = _dbcontext.VStockRequest.Where(w => w.ProcessTypeID == 2 && w.StatusID == 2).OrderBy(o => o.RequestNo).ToList();
                }
                else if (id == 2)
                {
                    resultdata = _dbcontext.VStockRequest.Where(w => w.ProcessTypeID == 2 && w.StatusID == 4).OrderBy(o => o.RequestNo).ToList();
                }
                else if (id == 3)
                {
                    resultdata = _dbcontext.VStockRequest.Where(w => w.ProcessTypeID == 2 && w.StatusID == 5).OrderBy(o => o.RequestNo).ToList();
                }
                else
                {
                    return null;
                }

                foreach (var item in resultdata)
                {
                    item.VStockRequestTrans = _dbcontext.VStockRequestTrans.Where(w => w.StockRequestID == item.StockRequestID).ToList();
                }

                _auditLogger.SaveActionLog("VStockRequest", ActionType.ListData, null, id, null, "HologramPunchingServiceRepository.GetStockRequestList()");
                return resultdata;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "HologramPunchingServiceRepository.GetStockRequestList()");
                return new List<VStockRequest>();
            }
        }

        public List<VWareHouse> GetWarehouseList()
        {
            try
            {
                var warehouseList = (from hp in _dbcontext.VHydrolicPressureCompleted
                                     join sr in _dbcontext.VStockRequest on hp.StockRequestID equals sr.StockRequestID
                                     join rl in _dbcontext.VRackLocation on hp.RackLocationID equals rl.RackLocationID
                                     join wh in _dbcontext.VWareHouse on rl.WareHouseID equals wh.WareHouseID
                                     where sr.StatusID == 5
                                     select wh).Distinct().ToList();

                _auditLogger.SaveActionLog("VWareHouse", ActionType.ListData, null, null, null, "HologramPunchingServiceRepository.GetWarehouseList()");
                return warehouseList;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "HologramPunchingServiceRepository.GetWarehouseList()");
                return new List<VWareHouse>();
            }
        }

        public List<VHydrolicPressureCompleted> GetHologramPunchingByWarehouseID(int id, int ComponentTypeID)
        {
            try
            {
                var resultdata = (from v in _dbcontext.VHydrolicPressureCompleted
                                  join r in _dbcontext.VRackLocation on v.RackLocationID equals r.RackLocationID
                                  where r.WareHouseID == id && r.ComponentTypeID == ComponentTypeID
                                  select v).ToList();

                _auditLogger.SaveActionLog("VHydrolicPressureCompleted", ActionType.ListData, null, null, null, "HologramPunchingServiceRepository.GetHologramPunchingByWarehouseID()");
                return resultdata;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "HologramPunchingServiceRepository.GetHologramPunchingByWarehouseID()");
                return new List<VHydrolicPressureCompleted>();
            }
        }

        public HologramDataResponse GetHologramDetailsAsync(int backstockid, int stockrequestid)
        {
            try
            {
                var dataResult = GetHologramDetails(backstockid, stockrequestid);

                _auditLogger.SaveActionLog("HologramDataResponse", ActionType.ListData, null, null, null, "HologramPunchingServiceRepository.GetHologramDetailsAsync()");
                return new HologramDataResponse
                {
                    StockRequests = dataResult.StockRequests?.ToList<VStockRequest>() ?? new List<VStockRequest>(),
                    VStockRequestTrans = dataResult.VStockRequestTrans?.ToList<VStockRequestTrans>() ?? new List<VStockRequestTrans>(),
                    HologramPunching = dataResult.HologramPunching?.ToList<VHologramPunching>() ?? new List<VHologramPunching>(),
                    BatchStock = dataResult.BatchStock?.ToList<BatchStock>() ?? new List<BatchStock>()
                };
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "HologramPunchingServiceRepository.GetHologramDetailsAsync()");
                return new HologramDataResponse();
            }
        }

        public FullHologramDataResult GetHologramDetails(int backstockid, int stockrequestid)
        {
            try
            {
                var result = new FullHologramDataResult();

                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                using (var command = new SqlCommand("GetHologramPunchingDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FK_BatchStockID", backstockid);
                    command.Parameters.AddWithValue("@FK_StockRequestID", stockrequestid);

                    var adapter = new SqlDataAdapter(command);
                    var dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    if (dataSet.Tables.Count > 0) result.StockRequests = dataSet.Tables[0];
                    if (dataSet.Tables.Count > 1) result.VStockRequestTrans = dataSet.Tables[1];
                    if (dataSet.Tables.Count > 2) result.HologramPunching = dataSet.Tables[2];
                    if (dataSet.Tables.Count > 3) result.BatchStock = dataSet.Tables[3];
                }

                _auditLogger.SaveActionLog("FullHologramDataResult", ActionType.ListData, null, null, null, "HologramPunchingServiceRepository.GetHologramDetails()");
                return result;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "HologramPunchingServiceRepository.GetHologramDetails()");
                return new FullHologramDataResult();
            }
        }

        public Tuple<bool, bool> DeleteHologramPunching(int HologramPunchingID)
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
                        command.CommandText = StoredProcedure.DELETEHOLOGRAMPUNCHING;
                        command.CommandType = CommandType.StoredProcedure;

                        var param = command.CreateParameter();
                        param.ParameterName = "@PK_HologramPunchingID";
                        param.Value = HologramPunchingID;

                        command.Parameters.Add(param);
                        command.ExecuteNonQuery();
                        IsSuccess = true;
                    }
                }

                _auditLogger.SaveActionLog("HologramPunching", ActionType.Delete, null, HologramPunchingID, null, "HologramPunchingServiceRepository.DeleteHologramPunching()");
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "HologramPunchingServiceRepository.DeleteHologramPunching()");
            }
            return Tuple.Create(IsSuccess, IsError);
        }

        public Tuple<bool, bool> CompleteHologramPunching(int StockRequestID)
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
                        command.CommandText = StoredProcedure.COMPLETEHOLOGRAMPUNCHING;
                        command.CommandType = CommandType.StoredProcedure;

                        var param = command.CreateParameter();
                        param.ParameterName = "@FK_BatchStockID";
                        param.Value = StockRequestID;

                        command.Parameters.Add(param);
                        command.ExecuteNonQuery();
                        IsSuccess = true;
                    }
                }

                _auditLogger.SaveActionLog("HologramPunching", ActionType.ListData, null, StockRequestID, null, "HologramPunchingServiceRepository.CompleteHologramPunching()");
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "HologramPunchingServiceRepository.CompleteHologramPunching()");
            }
            return Tuple.Create(IsSuccess, IsError);
        }

        public List<VHologramPunchingCompleted> GetHologramPunchingCompleted()
        {
            try
            {
                var resultdata = _dbcontext.VHologramPunchingCompleted.ToList();
                _auditLogger.SaveActionLog("VHologramPunchingCompleted", ActionType.ListData, null, null, null, "HologramPunchingServiceRepository.GetHologramPunchingCompleted()");
                return resultdata;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "HologramPunchingServiceRepository.GetHologramPunchingCompleted()");
                return new List<VHologramPunchingCompleted>();
            }
        }

        public VHologramPunchingCompleted GetHologramPunchingByBatchstockID(int BatchStockID)
        {
            try
            {
                var resultdata = _dbcontext.VHologramPunchingCompleted.FirstOrDefault(w => w.OutputBatchStockID == BatchStockID);
                _auditLogger.SaveActionLog("VHologramPunchingCompleted", ActionType.ListData, null, BatchStockID, null, "HologramPunchingServiceRepository.GetHologramPunchingByBatchstockID()");
                return resultdata;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "HologramPunchingServiceRepository.GetHologramPunchingByBatchstockID()");
                return new VHologramPunchingCompleted();
            }
        }


    }
}

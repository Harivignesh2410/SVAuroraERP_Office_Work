namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Production
{
    public class LaserNoMarkingServiceRepository : ILaserNoMarkingServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<LaserNoMarkingServiceRepository> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public LaserNoMarkingServiceRepository(SVAuroraERPDbContext dbcontext, ILogger<LaserNoMarkingServiceRepository> logger, IAuditLogger auditLogger, IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public UpdateResult SaveLaserNoMarking(LaserNoMarking request)
        {
            try
            {
                var result = new UpdateResult();

                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.INSERTLASERNOMARKING;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@FK_InputBatchStockID", request.InputBatchStockID);
                        command.Parameters.AddWithValue("@FK_ItemID", request.ItemID);
                        command.Parameters.AddWithValue("@FK_RackLocationID", request.RackLocationID);
                        command.Parameters.AddWithValue("@FK_MachineID", request.MachineID);
                        command.Parameters.AddWithValue("@FK_OperatorID", request.OperatorID);
                        command.Parameters.AddWithValue("@StartTime", request.StartTime);
                        command.Parameters.AddWithValue("@EndTime", request.EndTime);
                        command.Parameters.AddWithValue("@ProductionDate", request.ProductionDate);
                        command.Parameters.AddWithValue("@StartingNo", request.StartingNo);
                        command.Parameters.AddWithValue("@EndingNo", request.EndingNo);
                        command.Parameters.AddWithValue("@NoOfPlate", request.NoOfPlate);
                        command.Parameters.AddWithValue("@RejectedPlate", request.RejectedPlate);
                        command.Parameters.AddWithValue("@StartingLaserNo", request.StartingLaserNo);
                        command.Parameters.AddWithValue("@EndingLaserNo", request.EndingLaserNo);
                        command.Parameters.AddWithValue("@LastUpdatedBy", request.LastUpdatedBy);

                        // Output parameter for new ID
                        var pkLaserNoMarkingIDParam = command.CreateParameter();
                        pkLaserNoMarkingIDParam.ParameterName = "@PK_LaserNoMarkingID";
                        pkLaserNoMarkingIDParam.Direction = System.Data.ParameterDirection.Output;
                        pkLaserNoMarkingIDParam.DbType = System.Data.DbType.Int32;
                        command.Parameters.Add(pkLaserNoMarkingIDParam);

                        try
                        {
                            command.ExecuteNonQuery();

                            int id = (int)pkLaserNoMarkingIDParam.Value;

                            if (id > 0)
                            {
                                request.LaserNoConsumption.LaserNoMarkingID = id;
                                var saveResult = SaveLaserNoConsumption(request.LaserNoConsumption);
                                result.IsSuccess = true;
                            }
                            else
                            {
                                result.IsError = true;
                                result.IsSuccess = false;
                                result.ErrorMessage = "Duplicate entry or invalid range.";
                            }
                        }
                        catch (SqlException ex)
                        {
                            result.IsSuccess = false;
                            result.IsError = true;
                            result.ErrorMessage = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            result.IsSuccess = false;
                            result.IsError = true;
                            result.ErrorMessage = "Unexpected error: " + ex.Message;
                        }
                    }
                }
                _auditLogger.SaveActionLog("LaserNoMarking", ActionType.Insert, request.LaserNoConsumption.LaserNoMarkingID.ToString(), request,null, "LaserNoMarkingServiceRepository.SaveLaserNoMarking()");
                return result;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, request, "LaserNoMarkingServiceRepository.SaveLaserNoMarking()");
                return null;
            }
        }

        public UpdateResult UpdateLaserNoMarking(LaserNoMarking request)
        {
            try
            {
                var result = new UpdateResult();

                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.UPDATELASERNOMARKING;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@PK_LaserNoMarkingID", request.LaserNoMarkingID);
                        command.Parameters.AddWithValue("@FK_InputBatchStockID", request.InputBatchStockID);
                        command.Parameters.AddWithValue("@FK_ItemID", request.ItemID);
                        command.Parameters.AddWithValue("@FK_RackLocationID", request.RackLocationID);
                        command.Parameters.AddWithValue("@FK_MachineID", request.MachineID);
                        command.Parameters.AddWithValue("@FK_OperatorID", request.OperatorID);
                        command.Parameters.AddWithValue("@StartTime", request.StartTime);
                        command.Parameters.AddWithValue("@EndTime", request.EndTime);
                        command.Parameters.AddWithValue("@ProductionDate", request.ProductionDate);
                        command.Parameters.AddWithValue("@StartingNo", request.StartingNo);
                        command.Parameters.AddWithValue("@EndingNo", request.EndingNo);
                        command.Parameters.AddWithValue("@NoOfPlate", request.NoOfPlate);
                        command.Parameters.AddWithValue("@RejectedPlate", request.RejectedPlate);
                        command.Parameters.AddWithValue("@StartingLaserNo", request.StartingLaserNo);
                        command.Parameters.AddWithValue("@EndingLaserNo", request.EndingLaserNo);
                        command.Parameters.AddWithValue("@LastUpdatedBy", request.LastUpdatedBy);

                        try
                        {
                            command.ExecuteNonQuery();

                            request.LaserNoConsumption.LaserNoMarkingID = request.LaserNoMarkingID;
                            SaveLaserNoConsumption(request.LaserNoConsumption);

                            result.IsSuccess = true;
                        }
                        catch (SqlException ex)
                        {
                            result.IsSuccess = false;
                            result.IsError = true;
                            result.ErrorMessage = ex.Message; // Capture the SQL exception message
                        }
                        catch (Exception ex)
                        {
                            result.IsSuccess = false;
                            result.IsError = true;
                            result.ErrorMessage = "Unexpected error: " + ex.Message;
                        }
                    }
                }
                _auditLogger.SaveActionLog("LaserNoMarking", ActionType.Update, request.LaserNoConsumption.LaserNoMarkingID.ToString(), request,null, "LaserNoMarkingServiceRepository.UpdateLaserNoMarking()");
                return result;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, request, "LaserNoMarkingServiceRepository.UpdateLaserNoMarking()");
                return null;
            }
        }

        public Tuple<bool, bool> SaveLaserNoConsumption(LaserNoConsumption request)
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
                        command.CommandText = StoredProcedure.INSERTLASERCONSUMPTION;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var fkLaserNoMarkingIDParam = command.CreateParameter();
                        fkLaserNoMarkingIDParam.ParameterName = "@FK_LaserNoMarkingID";
                        fkLaserNoMarkingIDParam.Value = request.LaserNoMarkingID;

                        var fkBatchStockIDParam = command.CreateParameter();
                        fkBatchStockIDParam.ParameterName = "@FK_BatchStockID";
                        fkBatchStockIDParam.Value = request.BatchStockID;

                        var actualConsumedQtyParam = command.CreateParameter();
                        actualConsumedQtyParam.ParameterName = "@ActualConsumedQty";
                        actualConsumedQtyParam.Value = request.ActualConsumedQty;

                        var wastageQtyParam = command.CreateParameter();
                        wastageQtyParam.ParameterName = "@WastageQty";
                        wastageQtyParam.Value = request.WastageQty;

                        var wastagePercentageParam = command.CreateParameter();
                        wastagePercentageParam.ParameterName = "@WastagePercentage";
                        wastagePercentageParam.Value = request.WastagePercentage;

                        var balanceQtyParam = command.CreateParameter();
                        balanceQtyParam.ParameterName = "@BalanceQty";
                        balanceQtyParam.Value = request.BalanceQty;

                        // Add parameters to the command
                        command.Parameters.Add(fkLaserNoMarkingIDParam);
                        command.Parameters.Add(fkBatchStockIDParam);
                        command.Parameters.Add(actualConsumedQtyParam);
                        command.Parameters.Add(wastageQtyParam);
                        command.Parameters.Add(wastagePercentageParam);
                        command.Parameters.Add(balanceQtyParam);

                        // Execute
                        id = command.ExecuteNonQuery();

                        IsSuccess = true;
                    }

                }
                _auditLogger.SaveActionLog("LaserNoConsumption", ActionType.Insert, null, request,null, "LaserNoMarkingServiceRepository.SaveLaserNoConsumption()");
                return Tuple.Create(IsSuccess, IsError);
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, request, "LaserNoMarkingServiceRepository.SaveLaserNoConsumption()");
                return null;
            }
        }
        public List<VWareHouse> GetWarehouseList()
        {
            try
            {
                var warehouseList = (from hp in _dbcontext.VHologramPunchingCompleted
                                     join sr in _dbcontext.VStockRequest
                                         on hp.StockRequestID equals sr.StockRequestID
                                     join rl in _dbcontext.VRackLocation
                                         on hp.RackLocationID equals rl.RackLocationID
                                     join wh in _dbcontext.VWareHouse
                                         on rl.WareHouseID equals wh.WareHouseID
                                     select wh)
                                     .Distinct()
                                     .ToList();

                _auditLogger.SaveActionLog("VWareHouse", ActionType.ListData, null, null, null, "LaserNoMarkingServiceRepository.GetWarehouseList()");
                return warehouseList;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "LaserNoMarkingServiceRepository.GetWarehouseList()");
                return null;
            }
        }
        public List<VHologramPunchingCompleted> GetHologramPunchingByWarehouseID(int id, int ComponentTypeID)
        {
            try
            {
                var resultdata = (from v in _dbcontext.VHologramPunchingCompleted
                                  where v.StatusID <= 3
                                  join r in _dbcontext.VRackLocation
                                      on v.RackLocationID equals r.RackLocationID
                                  where r.WareHouseID == id && r.ComponentTypeID == ComponentTypeID
                                  select v)
                                 .ToList();
                _auditLogger.SaveActionLog("VHologramPunchingCompleted", ActionType.ListData, null, id,null,"LaserNoMarkingServiceRepository.GetHologramPunchingByWarehouseID()");
                return resultdata;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, id, "LaserNoMarkingServiceRepository.GetHologramPunchingByWarehouseID()");
                return null;
            }
        }
        public LaserDataResponse GetLaserNoAsync(int backstockid)
        {
            try
            {
                var dataResult = GetLaserNoDetails(backstockid);
                _auditLogger.SaveActionLog("LaserDataResponse", ActionType.ListData, null, backstockid,null, "LaserNoMarkingServiceRepository.GetLaserNoAsync()");
                return new LaserDataResponse
                {
                    LaserNoMarking = dataResult.LaserNoMarking?.ToList<VLaserNoMarking>() ?? new List<VLaserNoMarking>(),
                    VHologramPunchingCompleted = dataResult.VHologramPunchingCompleted?.ToList<VHologramPunchingCompleted>() ?? new List<VHologramPunchingCompleted>()
                };
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, backstockid, "LaserNoMarkingServiceRepository.GetLaserNoAsync()");
                return null;
            }
        }
        public FullLaserDataResult GetLaserNoDetails(int backstockid)
        {
            var result = new FullLaserDataResult();
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                using (var command = new SqlCommand("GetLaserNoMarkingDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FK_BatchStockID", backstockid);

                    var adapter = new SqlDataAdapter(command);
                    var dataSet = new DataSet();

                    adapter.Fill(dataSet); // Fills all result sets

                    // Convert each table into List<T> or DataTable
                    if (dataSet.Tables.Count > 0) result.LaserNoMarking = dataSet.Tables[0];
                    if (dataSet.Tables.Count > 1) result.VHologramPunchingCompleted = dataSet.Tables[1];
                }
                _auditLogger.SaveActionLog("FullLaserDataResult", ActionType.ListData, null, backstockid,null, "LaserNoMarkingServiceRepository.GetLaserNoDetails()");
                return result;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, backstockid, "LaserNoMarkingServiceRepository.GetLaserNoDetails()");
                return null;
            }
        }
        public Tuple<bool, bool> DeleteLaserNoMarking(int LaserNoMarkingID)
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
                        command.CommandText = StoredProcedure.DELETELASERNOMARKING;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var LaserNoMarkingIDParam = command.CreateParameter();
                        LaserNoMarkingIDParam.ParameterName = "@PK_LaserNoMarkingID";
                        LaserNoMarkingIDParam.Value = LaserNoMarkingID;

                        command.Parameters.Add(LaserNoMarkingIDParam);

                        command.ExecuteNonQuery();
                        IsSuccess = true;
                    }
                }
                _auditLogger.SaveActionLog("LaserNoMarking", ActionType.Delete, null, LaserNoMarkingID,null,"LaserNoMarkingServiceRepository.DeleteLaserNoMarking()");
                return Tuple.Create(IsSuccess, IsError);
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, LaserNoMarkingID, "LaserNoMarkingServiceRepository.DeleteLaserNoMarking()");
                return null;
            }
        }
        public Tuple<bool, bool> CompleteLaserNoMarking(int BatchStockID)
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
                        command.CommandText = StoredProcedure.COMPLETELASERNOMARKING;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var backstockIDParam = command.CreateParameter();
                        backstockIDParam.ParameterName = "@FK_BatchStockID";
                        backstockIDParam.Value = BatchStockID;

                        command.Parameters.Add(backstockIDParam);

                        command.ExecuteNonQuery();
                        IsSuccess = true;
                    }
                }
                _auditLogger.SaveActionLog("LaserNoMarking", ActionType.Update, null, BatchStockID,null,"LaserNoMarkingServiceRepository.CompleteLaserNoMarking()");
                return Tuple.Create(IsSuccess, IsError);
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, BatchStockID, "LaserNoMarkingServiceRepository.CompleteLaserNoMarking()");
                return null;
            }
        }
        public List<VLaserNoMarking> GetLaserNoMarkingCompleted()
        {
            try
            {
                var resultdata = _dbcontext.VLaserNoMarking.Where(w => w.StatusID != 4).ToList();
                _auditLogger.SaveActionLog("LaserNoMarking", ActionType.ListData, null, null, null, "LaserNoMarkingServiceRepository.GetLaserNoMarkingCompleted()"); 
                return resultdata;
              
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "LaserNoMarkingServiceRepository.GetLaserNoMarkingCompleted()");
                return null;
            }
        }

        public int GetLaserNoMarkingNxtNo()
        {
            try
            {
                var lastEndingNo = _dbcontext.VLaserNoMarking.Where(W=>W.NoOfPlate>0)
                                .OrderByDescending(x => x.LaserNoMarkingID)
                                .Select(x => x.EndingNo)
                                .FirstOrDefault();
            _auditLogger.SaveActionLog("LaserNoMarking", ActionType.Update, null, null,null, "LaserNoMarkingServiceRepository.GetLaserNoMarkingCompleted()");
                return lastEndingNo + 1;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "LaserNoMarkingServiceRepository.GetLaserNoMarkingCompleted()");
                return 0;
            }
           
        }


    }
}
